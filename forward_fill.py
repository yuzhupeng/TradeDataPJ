#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""forward fill: delete last incomplete bar, fetch latest data from aggr.trade, insert to MySQL"""

import requests
import mysql.connector
import json
from datetime import datetime, timezone

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
    {'name': 'minute', 'table': 'coin_exchagedatabyonemin', 'interval': '60000'},
    {'name': 'hour',   'table': 'coin_exchagedatabyhour',  'interval': '14400000'},
    {'name': 'day',    'table': 'coin_exchagedatabyday',   'interval': '86400000'},
]

def get_conn():
    return mysql.connector.connect(**DB_CONFIG)

def ts_ms(dt):
    return int(dt.timestamp() * 1000)

def main():
    conn = get_conn()
    cur = conn.cursor()

    for tf in TIMEFRAMES:
        name = tf['name']
        table = tf['table']
        interval = tf['interval']

        # 1. query newest record time
        cur.execute(f"SELECT MAX(Times) FROM {table}")
        row = cur.fetchone()
        newest = row[0]
        if newest is None:
            print(f"[{name}] 表为空，跳过")
            continue

        # 2. delete incomplete current bar
        cur.execute(f"DELETE FROM {table} WHERE Times = %s", (newest,))
        deleted = cur.rowcount
        conn.commit()
        print(f"[{name}] 删除最新时间 {newest} 数据: {deleted} 行")

        # 3. fetch from newest to now
        start_ms = int(newest.timestamp() * 1000)
        now_ms = ts_ms(datetime.now(timezone.utc))
        url = f"https://api.aggr.trade/historical/{start_ms}/{now_ms}/{interval}/{EXCHANGE_LIST}"
        print(f"[{name}] 请求: {newest} → now ({interval}ms)")

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
            print(f"[{name}] API请求失败: {e}")
            continue

        results = data.get('results', [])
        if not results:
            print(f"[{name}] API返回空")
            continue

        # 4. parse array format items only (skip object sub-minute data)
        rows = []
        for item in results:
            if isinstance(item, list) and len(item) >= 12:
                try:
                    ts = int(item[0] or 0)
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

                    import uuid
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
                    print(f"[{name}] 解析行异常: {e}")

        if not rows:
            print(f"[{name}] 无数据可写入")
            continue

        # 5. insert with ON DUPLICATE KEY UPDATE
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

        cur.executemany(sql, rows)
        conn.commit()
        print(f"[{name}] 写入 {len(rows)} 行")

    cur.close()
    conn.close()
    print("=== 全部完成 ===")

if __name__ == '__main__':
    main()
