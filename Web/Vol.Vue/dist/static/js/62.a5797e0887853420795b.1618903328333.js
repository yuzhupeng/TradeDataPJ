webpackJsonp([62],{"+xo/":function(t,s){},"/dEI":function(t,s,e){"use strict";Object.defineProperty(s,"__esModule",{value:!0});var i={data:function(){return{activedIndex:0,q_moel:!1,activeName:"1",q_items:[{title:"编辑或查询弹出框空白",desc:"请配置编辑行或查询行,同时点击[生成vue页面],如果配置是编辑行，同时需要点击[生成model]"},{title:"编辑或新建,保存后数据没有变化",desc:"1、确认是否设置了编辑行；2、修改编辑行后需要点击【生成model】与生成vue页面，如果主从表，在主表上点击生成vue页面"},{title:"点击[生成业务类]异常",desc:"请双击运行后台项目.../VOL.WebApi/builder_run.bat命令,只有生成业务类时才需要运行此命令，其他操作运行dev_run.bat即可"},{title:"代码生成后,打开页面报错",desc:"代码生成后，如果页面报错，请确认后台运行的是.../VOL.WebApi/dev_run.bat命令"},{title:"select/selectList编辑时不能绑定默认值",desc:"原因在于如果自定义sql的key是数字与数据库字段的类型不一致时就可能导致绑定失败；解决办法：将自定义sql里key字段转换成字符串。"}],nav:["可实现功能","常见问题","准备工作","主从(明细)生成代码","1.开始生成代码","2.填写表或视图信息","3.配置表结构信息","4.查看生成完的代码","5.菜单配置","6.查看生成的页面","代码生成器参数配置","代码生成器表结构配置"],codeRequire:[{text:"先在数据库创建表，必须有主键,只能是自增Id或Guid，MySql数据库使用Guid请将字段设置为char长度36位"},{text:"表最好包括创建(修改)人、时间等字段，在新增或编辑时，框架会自动给这几个字段设置值"},{text:"字段格式要求参照后台项目appsettings.json中属性CreateMember、ModifyMember的说明"},{text:"参照项目启动,把项目跑起来"}]}},methods:{preview:function(t){window.open(t)},to:function(t){if(this.activedIndex=t,1!=t){var s=document.getElementById("doc-"+t).offsetTop-100;0==t&&(s=0),window.scrollTo(0,s)}else this.q_moel=!0}},created:function(){}},c={render:function(){var t=this,s=t.$createElement,e=t._self._c||s;return e("div",{staticClass:"coder-container"},[e("div",{staticClass:"left"},t._l(t.nav,function(s,i){return e("ul",{key:i},[e("li",{class:{actived:t.activedIndex==i,step:i>3&&i<10},on:{click:function(s){t.to(i)}}},[t._v(t._s(s))])])}),0),t._v(" "),e("div",{staticClass:"right"},[t._m(0),t._v(" "),e("div",{staticClass:"coder-doc",attrs:{id:"doc-1"}},[t._m(1),t._v(" "),e("ul",{staticClass:"coder-group"},t._l(t.codeRequire,function(s,i){return e("li",{key:i,staticClass:"coder-list"},[t._v(t._s(s.text))])}),0)]),t._v(" "),t._m(2),t._v(" "),e("br"),t._v(" "),e("br"),t._v(" "),e("h2",{staticStyle:{color:"red","font-weight":"500","margin-bottom":"20px"}},[t._v("1、创建mysql表，只有字段类型是guid时设置成char(36),其他字段的长度都不要设置成长度36，否则会替换成guid类型")]),t._v(" "),e("h2",{staticStyle:{color:"red","font-weight":"500","margin-bottom":"20px"}},[t._v("2、数据库字段不要设置类型bit(bool)，请用int或byte替代")]),t._v(" "),e("p",[e("Alert",{staticStyle:{"line-height":"2.5",width:"900px"}},[t._v("\n        生成代码时项目启动（必看）\n        "),e("template",{slot:"desc"},[e("p",[t._v("1、按项目启动文档启动项目即可")]),t._v(" "),e("p",{staticStyle:{color:"red"}},[t._v("2、后台请运行 ../VOL.WebApi/builder_run.bat命令，如果不需要生成业务类运行dev_run.bat即可(第一次生成某张表代码时候才需要运行builder_run.bat)")])])],2)],1),t._v(" "),e("h2",{staticStyle:{color:"red","font-weight":"500","margin-bottom":"15px"}},[t._v("生成代码后，查询框或弹出编辑框是空的，请看代码生成第3步说明")]),t._v(" "),e("h2",{staticStyle:{color:"red","font-weight":"500","margin-bottom":"15px"}},[t._v("生成代码后，打开页面异常，请看代码生成第6步说明")]),t._v(" "),e("div",{staticClass:"coder-doc",attrs:{id:"doc-3"}},[e("h1",{staticStyle:{"margin-bottom":"20px"}},[t._v("生成代码")]),t._v(" "),t._m(3),t._v(" "),e("ul",{staticClass:"coder-group"},[e("li",{staticClass:"coder-list"},[t._v("点击新建,弹出选择框，如果只是做修改跳过此步，直接修改页面配置后点保存，再点各种生成操作")]),t._v(" "),e("li",{staticClass:"coder-img",on:{click:function(s){t.preview("https://imgs-1256993465.cos.ap-chengdu.myqcloud.com/doc/step1.png")}}},[e("img",{attrs:{src:"https://imgs-1256993465.cos.ap-chengdu.myqcloud.com/doc/step1.png"}})])])]),t._v(" "),e("div",{staticClass:"coder-doc",attrs:{id:"doc-4"}},[t._m(4),t._v(" "),e("ul",{staticClass:"coder-group"},[e("li",{staticClass:"coder-list"},[t._v("点击确认，自动从后台加载表结构信息(如果只是生成空菜单,里面就随便填)")]),t._v(" "),e("li",{staticClass:"coder-list"},[t._v("项目命名空间：代码生成时所放在类库(现框架采用一个模块为一个类库，可自行决定是否需要增加类库)")]),t._v(" "),e("li",{staticClass:"coder-list"},[t._v("项目文件夹：生成的文件放在文件夹,此文件夹由代码生成器创建,不需要手动创建")]),t._v(" "),e("li",{staticClass:"coder-list"},[t._v("表名:可以是视图或表,名字必须和数据库一样")]),t._v(" "),e("li",{staticClass:"coder-list"},[t._v("如果只想创建一个空菜单，上面表名随便填写")]),t._v(" "),e("li",{staticClass:"coder-img",on:{click:function(s){t.preview("https://imgs-1256993465.cos.ap-chengdu.myqcloud.com/doc/step2.png")}}},[e("img",{attrs:{src:"https://imgs-1256993465.cos.ap-chengdu.myqcloud.com/doc/step2.png"}})])])]),t._v(" "),e("div",{staticClass:"coder-doc",attrs:{id:"doc-5"}},[t._m(5),t._v(" "),e("ul",{staticClass:"coder-group"},[e("li",{staticClass:"coder-list",staticStyle:{color:"red"}},[t._v("根据需要配置下面表格中的查询与新建、编辑信息 (不设置编辑、新建行，编辑或查询时，弹出框是空白的)")]),t._v(" "),e("li",{staticClass:"coder-list"},[t._v("表别名：别名将替代原表名生成的Model与业务类。一个表只能有一个别名，默认表名与别名相同。若用别名,必须将已经生成文件删除")]),t._v(" "),e("li",{staticClass:"coder-list"},[t._v("点击保存、生成Vue页面、生成Model、生成业务类即可(每次修改信息后都需要点击保存)。")]),t._v(" "),e("li",{staticClass:"coder-list",staticStyle:{color:"#0b906d"}},[t._v("Vue视图绝对路径：生成Vue页面必须指定此路径，路径为当前Vue项目的views文件夹，如E:/VOL.Vue/src/views")]),t._v(" "),e("li",{staticClass:"coder-img",on:{click:function(s){t.preview("https://imgs-1256993465.cos.ap-chengdu.myqcloud.com/doc/sep3.png")}}},[e("img",{attrs:{src:"https://imgs-1256993465.cos.ap-chengdu.myqcloud.com/doc/sep3.png"}})])])]),t._v(" "),t._m(6),t._v(" "),e("div",{staticClass:"coder-doc",attrs:{id:"doc-7"}},[t._m(7),t._v(" "),e("ul",{staticClass:"coder-group"},[e("li",{staticClass:"coder-list"},[t._v("Url:通过VsCode打开vue项目,找到router文件夹下viewGird.js找当前生成表的path属性/SellOrder就是配置菜单需要配置的url,直接复制过来即可")]),t._v(" "),e("li",{staticClass:"coder-list"},[t._v("表名:在生成代码时填写的表名或视图名，必须一致，否则权限验证通不过")]),t._v(" "),e("li",{staticClass:"coder-img",on:{click:function(s){t.preview("https://imgs-1256993465.cos.ap-chengdu.myqcloud.com/doc/sep5.png")}}},[e("img",{attrs:{src:"https://imgs-1256993465.cos.ap-chengdu.myqcloud.com/doc/sep5.png"}})])])]),t._v(" "),e("div",{staticClass:"coder-doc",attrs:{id:"doc-8"}},[t._m(8),t._v(" "),e("ul",{staticClass:"coder-group"},[e("li",{staticClass:"coder-list",staticStyle:{color:"red"}},[t._v("确认后台项目运行的是路径 …/VOL.WebApi/dev_run.bat 文件,")]),t._v(" "),e("li",{staticClass:"coder-list"},[t._v("输入http://localhost:8080/sellOrder")]),t._v(" "),e("li",{staticClass:"coder-img",on:{click:function(s){t.preview("https://imgs-1256993465.cos.ap-chengdu.myqcloud.com/doc/sep6.png")}}},[e("img",{attrs:{src:"https://imgs-1256993465.cos.ap-chengdu.myqcloud.com/doc/sep6.png"}})])])]),t._v(" "),t._m(9),t._v(" "),t._m(10),t._v(" "),t._m(11),t._v(" "),t._m(12),t._v(" "),e("br"),t._v(" "),e("br")]),t._v(" "),e("Drawer",{staticClass:"q-drawer",attrs:{width:800,title:"代码生成器使用常见问题",closable:!1},model:{value:t.q_moel,callback:function(s){t.q_moel=s},expression:"q_moel"}},[e("Alert",{attrs:{type:"success","show-icon":""}},[t._v("\n      关于生成model\n      "),e("template",{slot:"desc"},[e("p",[t._v("如果修改了编辑行或者编辑列，必须点生成model；如果只允许编辑，但不想显示出来，编辑行设置为0，再点生成model")]),t._v(" "),e("p",[t._v("框架不支持多主键，如果有多个主键，在生成页面，主键列只勾选一个即可")])])],2),t._v(" "),e("el-collapse",{attrs:{accordion:""},model:{value:t.activeName,callback:function(s){t.activeName=s},expression:"activeName"}},t._l(t.q_items,function(s,i){return e("el-collapse-item",{key:i,attrs:{title:s.title,name:i+1}},[e("div",[t._v(t._s(s.desc))])])}),1)],1)],1)},staticRenderFns:[function(){var t=this,s=t.$createElement,e=t._self._c||s;return e("div",{staticClass:"coder-doc",attrs:{id:"doc-0"}},[e("div",{staticClass:"title"},[e("h2",[t._v("使用代码生成器可实现的功能")])]),t._v(" "),e("ul",{staticClass:"coder-group"},[e("li",{staticClass:"coder-list"},[t._v("代码生成器可自动完成基本功能的实现,不需要写代码,包括：")]),t._v(" "),e("li",{staticClass:"coder-list"},[t._v("单表/主从表：查询、删除、新增、修改、导入、导出、审核、表单/table数据源自动绑定")]),t._v(" "),e("li",{staticClass:"coder-list"},[t._v("单表/主从表自动生成前端及后台代码,不需要写任何代码,并支持前后端扩展实现复杂的功能")]),t._v(" "),e("li",{staticClass:"coder-list"},[t._v("一对多暂未实现,需要自己写扩展")])])])},function(){var t=this.$createElement,s=this._self._c||t;return s("div",{staticClass:"title"},[s("h2",[this._v("使用代码生成器前需要准备的工作")])])},function(){var t=this,s=t.$createElement,e=t._self._c||s;return e("div",{staticClass:"coder-doc",attrs:{id:"doc-2"}},[e("div",{staticClass:"title"},[e("h2",[t._v("主从(明细)表生成代码需要注意")])]),t._v(" "),e("ul",{staticClass:"coder-group"},[e("li",{staticClass:"coder-list"},[t._v("明细表的外键必须是主表的主键字段,可参照SellOrder的明细表SellOrderList的外建")]),t._v(" "),e("li",{staticClass:"coder-list"},[t._v("主从表代码生成步骤都是一样，在生成主表前需要先将从表按现有步骤生成")]),t._v(" "),e("li",{staticClass:"coder-list"},[t._v("在主表生成配置页面填上【明细表名】与【明细表中文名】,点击生成vue页面、model、业务类即可完成主从页面代码的生成")]),t._v(" "),e("li",{staticClass:"coder-list"},[t._v("代码生成器中可以不用生成从表vue页面")])])])},function(){var t=this.$createElement,s=this._self._c||t;return s("div",{staticClass:"title"},[s("h2",[this._v("1、选择菜单：在线代生成器->Vue+后台代码生成")])])},function(){var t=this.$createElement,s=this._self._c||t;return s("div",{staticClass:"title"},[s("h2",[this._v("2、填写需要生成表或视图的信息")])])},function(){var t=this.$createElement,s=this._self._c||t;return s("div",{staticClass:"title"},[s("h2",[this._v("3、配置表结构信息")])])},function(){var t=this.$createElement,s=this._self._c||t;return s("div",{staticClass:"coder-doc",attrs:{id:"doc-6"}},[s("div",{staticClass:"title"},[s("h2",[this._v("4、查看生成完的代码")])]),this._v(" "),s("ul",{staticClass:"coder-group"},[s("li",{staticClass:"coder-list"},[this._v("生成完成后在vs中搜索当前表就能看到生成的代码了")]),this._v(" "),s("li",{staticClass:"coder-list"},[this._v("vue代码也同时生成了,可在vscode中搜索当前文件(文件名都是以当前表名开头)")]),this._v(" "),s("li",{staticClass:"coder-img"},[s("img",{staticStyle:{width:"300px"},attrs:{src:"https://imgs-1256993465.cos.ap-chengdu.myqcloud.com/doc/sep4.png"}}),this._v(" "),s("img",{staticStyle:{width:"300px"},attrs:{src:"https://imgs-1256993465.cos.ap-chengdu.myqcloud.com/doc/sep4x.png"}})])])])},function(){var t=this.$createElement,s=this._self._c||t;return s("div",{staticClass:"title"},[s("h2",[this._v("5、菜单配置")])])},function(){var t=this.$createElement,s=this._self._c||t;return s("div",{staticClass:"title"},[s("h2",[this._v("6、查看生成的页面")])])},function(){var t=this.$createElement,s=this._self._c||t;return s("div",{attrs:{id:"doc-9"}},[s("h1",{staticStyle:{padding:"15px 0","font-weight":"500"}},[this._v("代码生成器参数配置")])])},function(){var t=this,s=t.$createElement,e=t._self._c||s;return e("table",[e("tbody",[e("tr",[e("td",[t._v("字段")]),t._v(" "),e("td",[t._v("描述")])]),t._v(" "),e("tr",[e("td",[t._v("Id")]),t._v(" "),e("td",[t._v("表Id")])]),t._v(" "),e("tr",[e("td",[t._v("父级Id")]),t._v(" "),e("td",[t._v("表所放在位置")])]),t._v(" "),e("tr",[e("td",[t._v("项目命名空间")]),t._v(" "),e("td",[t._v("将当前表生成的文件放在所选命名空间的类库中")])]),t._v(" "),e("tr",[e("td",[t._v("表中文名")]),t._v(" "),e("td",[t._v("需要生成代码的表具体名")])]),t._v(" "),e("tr",[e("td",[t._v("表别名")]),t._v(" "),e("td",[t._v("表别名：如果不想暴露真实表名，可以自行设置任何表别名，表别名将替代原表名生成的Model与业务类。一个表只能有一个别名，默认表名与别名相同，如果想使用别名，必须将已经生成过的文件删除")])]),t._v(" "),e("tr",[e("td",[t._v("实际表名")]),t._v(" "),e("td",[t._v("用实际表名替换表名, 具体功能未开发")])]),t._v(" "),e("tr",[e("td",[t._v("项目文件夹")]),t._v(" "),e("td",[t._v("将当前表生成的文件放在所选命名空间类库下的文件夹( 不需要人为创建)")])]),t._v(" "),e("tr",[e("td",[t._v("明细表中文名")]),t._v(" "),e("td",[t._v("明细从表的中文名")])]),t._v(" "),e("tr",[e("td",[t._v("明细表名")]),t._v(" "),e("td",[t._v("明细从表, 用于生成主从表关系及UI, 生成代码前, 必须先生成明细表代码")])]),t._v(" "),e("tr",[e("td",[t._v("快捷编辑字段")]),t._v(" "),e("td",[t._v("\n            设置[快捷编辑字段]后，前台界面表格点击此链接可快速查看详细信息 \n            "),e("img",{staticStyle:{"box-sizing":"border-box","-webkit-tap-highlight-color":"transparent","border-style":"none",width:"350px !important","margin-bottom":"30px","margin-top":"10px"},attrs:{height:"110",src:"https://imgs-1256993465.cos.ap-chengdu.myqcloud.com/doc/link.png"}})])]),t._v(" "),e("tr",[e("td",[t._v("排序字段")]),t._v(" "),e("td",[t._v("前台表格的排序字段，如果不是自增的主键，必须设置此值")])]),t._v(" "),e("tr",[e("td",[t._v("Vue 视图绝对路径")]),t._v(" "),e("td",[t._v("生成Vue 页面存放的位置，如：E:/project/views/ Vue 页面生成后会放在此路径下")])])])])},function(){var t=this.$createElement,s=this._self._c||t;return s("div",{attrs:{id:"doc-10"}},[s("h1",{staticStyle:{padding:"15px 0","font-weight":"500"}},[this._v("代码生成器表结构配置")])])},function(){var t=this,s=t.$createElement,e=t._self._c||s;return e("table",[e("tbody",[e("tr",[e("td",[t._v("字段")]),t._v(" "),e("td",[t._v("描述")])]),t._v(" "),e("tr",[e("td",[t._v("列中文名")]),t._v(" "),e("td",[t._v("表显示的中文名")])]),t._v(" "),e("tr",[e("td",[t._v("列名")]),t._v(" "),e("td",[t._v("表列名")])]),t._v(" "),e("tr",[e("td",[t._v("列最大长度")]),t._v(" "),e("td",[t._v("数据库设置的列长度 如果使用的mysql数据库并且主键使用的是Guid，数据库字段类型应该设置为char 长度为36，否则生成实体Model时会与数据库类型对应不上")])]),t._v(" "),e("tr",[e("td",[t._v("数据类型")]),t._v(" "),e("td",[t._v("C# 属性的数据类型( 除非数据库字段类型发生变, 其他不需要修改)")])]),t._v(" "),e("tr",[e("td",[t._v("table列显示类型")]),t._v(" "),e("td",[t._v("如果table的列存的是图片、excel或其他文件的路径，就选择此列的配置,如:列存的是为图片就选择img，如果是Excel文件的路径就选择excel，这一列在table上点击文件名时会自动下载文件")])]),t._v(" "),e("tr",[e("td",[t._v("可为空")]),t._v(" "),e("td",[t._v("表字段是否可为null, 此处会涉及前、后端验证规则，默认加载的是表结构")])]),t._v(" "),e("tr",[e("td",[t._v("排序号")]),t._v(" "),e("td",[t._v("前端页面表格显示的顺序")])]),t._v(" "),e("tr",[e("td",[t._v("数据源")]),t._v(" "),e("td",[t._v("如果字段对应的是下拉框或多选框，此处选择对应的数据源的字典编号, 在菜单：下拉框绑定设置中配置数据源，具体可参照现有配置")])]),t._v(" "),e("tr",[e("td",[t._v("是否只读")]),t._v(" "),e("td",[t._v("编辑或新建时，如果此字段为只读，则不可修改")])]),t._v(" "),e("tr",[e("td",[t._v("编辑行、编辑列")]),t._v(" "),e("td",[t._v("新建/编辑时，此字段所在的行与列，如行=1 ，列=2 ，则界面所在位置为第1 行第2 列(此配置直接决定表的编辑或新建字段,不在此配置中的字段，编辑或新建时都会被过滤移除) 编辑行修改后需要点击【生成model】,如果只想编辑，界面不想显示，编辑行设置为0")])]),t._v(" "),e("tr",[e("td",[t._v("编辑类型")]),t._v(" "),e("td",[t._v("新建/ 编辑时标签的类型，如日期标签，下拉框，text 等")])]),t._v(" "),e("tr",[e("td",[t._v("colSize")]),t._v(" "),e("td",[t._v("编辑、新建、查看时此字段显示的长度，如果设置的是12 则，此字段独占一行，可选值1-12")])]),t._v(" "),e("tr",[e("td",[t._v("查询行、查询列")]),t._v(" "),e("td",[t._v("查询时，此字段所在的行与列，如行=1 ，列=2 ，则界面所在位置为第1 行第2 列")])]),t._v(" "),e("tr",[e("td",[t._v("查询类型")]),t._v(" "),e("td",[t._v("查询时标签的类型，如日期标签，下拉框，text 等")])]),t._v(" "),e("tr",[e("td",[t._v("导入列、Api 输入列，是否可为空、api 输出列")]),t._v(" "),e("td",[t._v("具体业务未实现")])]),t._v(" "),e("tr",[e("td",[t._v("主键")]),t._v(" "),e("td",[t._v("\n            设置是否为主键，必须一个主键\n            "),e("br")])])])])}]};var l=e("VU/8")(i,c,!1,function(t){e("+xo/"),e("VZfm")},"data-v-1479bf94",null);s.default=l.exports},VZfm:function(t,s){}});