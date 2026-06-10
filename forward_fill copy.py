#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""forward fill: delete last incomplete bar, fetch latest data from aggr.trade, insert to MySQL
   also backfill historical data from earliest record backwards.
"""

import requests
import mysql.connector
import json
from datetime import datetime, timezone, timedelta
import uuid

DB_CONFIG = {
    'host': '127.0.0.1',
    'port': 3306,
    'user': 'root',
    'password': 'fishyue1',
    'database': 'coinwin_data',
    'charset': 'utf8mb4'
}

EXCHANGE_LIST = (
    "BINANCE_FUTURES%3Abtcusd_perp"
    "%2BBITFINEX%3ABTCUSD"
    "%2BBITMEX%3AXBTUSD"
    "%2BBYBIT%3ABTCUSD"
    "%2BCOINBASE%3ABTC-USD"
    "%2BDERIBIT%3ABTC-PERPETUAL"
    "%2BBINANCE%3Abtcusdt"
    "%2BBINANCE_FUTURES%3Abtcusdt"
    "%2BBITFINEX%3ABTCUST"
    "%2BBITFINEX%3ABTCF0%3AUSTF0"
    "%2BBITMEX%3AXBTUSDT"
    "%2BBYBIT%3ABTCUSDT"
    "%2BCOINBASE%3ABTC-USDT"
    "%2BBITSTAMP%3Abtcusd"
    "%2BOKEX%3ABTC-USD-SWAP"
    "%2BKRAKEN%3API_XBTUSD"
    "%2BOKEX%3ABTC-USDT-SWAP"
)

TIMEFRAMES = [
 
    # {'name': 'hour',   'table': 'coin_exchagedatabyhour',  'interval': '14400000'},
    {'name': 'day',    'table': 'coin_exchagedatabyday',   'interval': '86400000'},
]

# Backfill configuration
STEP_BARS = 5000          # max number of bars per API request
MAX_BACK_DAYS = 2000    # how many days back to fill at most

def get_conn():
    return mysql.connector.connect(**DB_CONFIG)

def ts_ms(dt):
    return int(dt.timestamp() * 1000)

def fetch_and_parse(start_ms, end_ms, interval_ms, exchange_list):
    """
    Fetch data from aggr.trade API and parse into rows ready for insertion.
    Returns:
        rows: list of tuples for INSERT
        min_time_ms: the minimum timestamp (ms) among returned data, or None if empty
    """
    url = f"https://api.aggr.trade/historical/{start_ms}/{end_ms}/{interval_ms}/{exchange_list}"
    try:
        resp = requests.get(url, timeout=120,
            headers={
                'User-Agent': 'Mozilla/5.0',
                'Origin': 'https://aggr.trade',
                'Referer': 'https://aggr.trade/',
            }
        )
        data = resp.json()
    except Exception as e:
        print(f"   API请求失败: {e}")
        return [], None

    results = data.get('results', [])
    if not results:
        return [], None

    rows = []
    min_time = None
    for item in results:
        if isinstance(item, list) and len(item) >= 12:
            try:
                ts = int(item[0] or 0)
                if min_time is None or ts < min_time:
                    min_time = ts

                t = datetime.fromtimestamp(ts, tz=timezone.utc)
                close_val = float(item[2] or 0)
                low_val = float(item[6] or 0) if item[6] is not None else 0
                high_val = float(item[4] or 0) if item[4] is not None else 0
                open_val = float(item[9] or 0) if item[9] is not None else 0
                lbuy = float(item[5] or 0) if item[5] is not None else 0
                lsell = float(item[7] or 0) if item[7] is not None else 0
                vbuy = float(item[10] or 0) if item[10] is not None else 0
                vsell = float(item[11] or 0) if item[11] is not None else 0
                cb = int(item[1] or 0)
                cs = int(item[3] or 0)
                market = str(item[8] or '')

                parts = market.split(':') if ':' in market else [market, '']
                exch = parts[0]
                pair = parts[1] if len(parts) > 1 else ''

                did = uuid.uuid4().hex[:12]

                rows.append((
                    did, t, str(ts),
                    round(close_val, 2),
                    cb + cs, cb, cs,
                    exch,
                    round(high_val, 2), round(low_val, 2), round(open_val, 2),
                    round(lbuy, 2), round(lsell, 2), round(lbuy + lsell, 2),
                    pair,
                    round(vbuy + vsell, 2), round(vbuy, 2), round(vsell, 2),
                    market,
                    'SYSTEM', datetime.now(), '@CLOSED'
                ))
            except Exception as e:
                print(f"   解析行异常: {e}")

    return rows, min_time

def backfill_history(conn, cur, tf):
    """
    Backfill older data from the earliest timestamp in the table,
    moving backwards until no more data or reaching MAX_BACK_DAYS limit.
    """
    name = tf['name']
    table = tf['table']
    interval_ms = int(tf['interval'])

    # Get the current earliest time in the table
    cur.execute(f"SELECT MIN(Times) FROM {table}")
    row = cur.fetchone()
    earliest = row[0]
    if earliest is None:
        print(f"[{name}] 表为空，无法向前回溯")
        return

    # Determine the boundary (oldest allowed timestamp)
    boundary_dt = datetime.now(timezone.utc) - timedelta(days=MAX_BACK_DAYS)
    boundary_ms = int(boundary_dt.timestamp() * 1000)

    current_end_ms = int(earliest.timestamp() * 1000)
    if current_end_ms <= boundary_ms:
        print(f"[{name}] 最早时间 {earliest} 已达边界 ({MAX_BACK_DAYS}天前)，无需回溯")
        return

    print(f"[{name}] 开始历史回溯: 当前最早 {earliest}, 目标边界 {boundary_dt}")

    total_inserted = 0
    step_ms = STEP_BARS * interval_ms
    loop_count = 0
    max_loops = 1000  # safety

    while loop_count < max_loops and current_end_ms > boundary_ms:
        start_ms = max(current_end_ms - step_ms, boundary_ms)
        print(f"  请求区间: {datetime.fromtimestamp(start_ms/1000, tz=timezone.utc)} → {datetime.fromtimestamp(current_end_ms/1000, tz=timezone.utc)}")

        rows, returned_min_ms = fetch_and_parse(start_ms, current_end_ms, interval_ms, EXCHANGE_LIST)

        if not rows:
            print(f"  [{name}] API 无数据返回，停止回溯")
            break

        # Filter rows that are older than current earliest (avoid overlap)
        new_rows = []
        oldest_in_batch = None
        for r in rows:
            ts_str = r[2]  # 'utime' field is string timestamp ms
            ts_int = int(ts_str)
            if ts_int < current_end_ms:
                new_rows.append(r)
                if oldest_in_batch is None or ts_int < oldest_in_batch:
                    oldest_in_batch = ts_int

        if new_rows:
            sql = f"""INSERT INTO {table}
                (DID, Times, utime, `close`, `count`, count_buy, count_sell, exchange,
                 high, low, `open`, liquidation_buy, liquidation_sell, liquidation,
                 pair, vol, vol_buy, vol_sell, Unit,
                 SYS_Createby, SYS_CreateDate, SYS_Status)
                VALUES (%s,%s,%s,%s,%s,%s,%s,%s, %s,%s,%s, %s,%s,%s, %s,%s,%s,%s,%s, %s,%s,%s)
                ON DUPLICATE KEY UPDATE
                 `close`=VALUES(`close`), high=VALUES(high), low=VALUES(low),
                 `open`=VALUES(`open`), vol=VALUES(vol), vol_buy=VALUES(vol_buy),
                 vol_sell=VALUES(vol_sell), liquidation=VALUES(liquidation),
                 liquidation_buy=VALUES(liquidation_buy), liquidation_sell=VALUES(liquidation_sell)"""
            cur.executemany(sql, new_rows)
            conn.commit()
            total_inserted += len(new_rows)
            print(f"  [{name}] 插入 {len(new_rows)} 行 (累计 {total_inserted})")
        else:
            print(f"  [{name}] 本批次无更早数据")

        # Update current_end_ms to the oldest timestamp we got, if any
        if returned_min_ms is not None and returned_min_ms < current_end_ms:
            current_end_ms = returned_min_ms
        else:
            # No progress, stop
            print(f"  [{name}] 时间无推进，停止回溯")
            break

        # If returned fewer bars than requested, likely reached the beginning of data
        if rows and len(rows) < STEP_BARS:
            print(f"  [{name}] 返回数量不足 {STEP_BARS}，可能已到达数据源头，停止回溯")
            break

        loop_count += 1

    print(f"[{name}] 历史回溯完成，共插入 {total_inserted} 行")

def main():
    conn = get_conn()
    cur = conn.cursor()

    for tf in TIMEFRAMES:
        name = tf['name']
        table = tf['table']
        interval = tf['interval']
 

        # -------- Backfill older data --------
        backfill_history(conn, cur, tf)

    cur.close()
    conn.close()
    print("=== 全部完成 ===")

if __name__ == '__main__':
    main()