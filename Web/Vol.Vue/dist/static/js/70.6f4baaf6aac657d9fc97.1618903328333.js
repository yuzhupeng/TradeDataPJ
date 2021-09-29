webpackJsonp([70],{TRBd:function(t,e){},kz9v:function(t,e,s){"use strict";Object.defineProperty(e,"__esModule",{value:!0});var i={render:function(){var t=this,e=t.$createElement,s=t._self._c||e;return s("div",[s("Alert",{attrs:{type:"success","show-icon":""}},[s("template",{slot:"desc"},[s("h4",{staticStyle:{color:"#2196F3"}},[s("p",[t._v("-- 此后台验证封装目的是为了增加代码复用性、尽量避免自己写if esle判断提交的参数。")])]),t._v(" "),s("h4",[s("p",[t._v("-- 支持简单实体、多参数验证，并可以指实体字段验证及自定义规则普通参数验证。")])]),t._v(" "),s("h4",[s("p",[t._v("-- 参数验证避免每个方法中写if else判断参数是否为空或合法，只需要在控制器上添加 [ObjectModelValidatorFilter(ValidatorModel.xxx)]或 [ObjectGeneralValidatorFilter(ValidatorGeneral.xxx)]即可完成实体或普通参数校验。")])]),t._v(" "),s("p",[t._v("1、后台实体复用：同一个实体，不同的接口验证的字段不同，需要注册对应字段。")]),t._v(" "),s("p",[t._v("2、普通多参数验证：普通参数可用于任何参数名相同的接口。")]),t._v(" "),s("p",[t._v("3、具体使用参照：ObjectActionValidatorExampleController，具体注入验证规则参照：ValidatorContainer.cs")])])],2),t._v(" "),s("br"),t._v(" "),s("div",{staticClass:"validator"},[s("div",{staticClass:"general"},[s("div",{staticClass:"v1"},[s("h2",[t._v("验证所有普通参数")]),t._v(" "),s("div",{staticClass:"v-input"},[t._m(0),t._v(" "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"输入用户名"},model:{value:t.userName1,callback:function(e){t.userName1=e},expression:"userName1"}})],1),t._v(" "),s("div",{staticClass:"v-input"},[t._m(1),t._v(" "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"输入手机号"},model:{value:t.phoneNo1,callback:function(e){t.phoneNo1=e},expression:"phoneNo1"}})],1),t._v(" "),s("div",{staticClass:"btn"},[s("Button",{attrs:{type:"success",long:""},on:{click:function(e){t.test1()}}},[t._v("提交验证test1")])],1)]),t._v(" "),s("div",{staticClass:"v2"},[s("h2",[t._v("只验证手机号")]),t._v(" "),s("div",{staticClass:"v-input"},[s("label",[t._v("用户名：")]),t._v(" "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"输入用户名"},model:{value:t.userName2,callback:function(e){t.userName2=e},expression:"userName2"}})],1),t._v(" "),s("div",{staticClass:"v-input"},[t._m(2),t._v(" "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"输入手机号"},model:{value:t.phoneNo2,callback:function(e){t.phoneNo2=e},expression:"phoneNo2"}})],1),t._v(" "),s("div",{staticClass:"btn"},[s("Button",{attrs:{type:"success",long:""},on:{click:function(e){t.test2()}}},[t._v("提交验证test2")])],1)]),t._v(" "),s("div",{staticClass:"v3"},[s("h2",[t._v("验证字符长度与数字大小")]),t._v(" "),s("div",{staticClass:"v-input"},[t._m(3),t._v(" "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"长度[6-10]"},model:{value:t.local,callback:function(e){t.local=e},expression:"local"}})],1),t._v(" "),s("div",{staticClass:"v-input"},[t._m(4),t._v(" "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"值范围[200-500]"},model:{value:t.qty,callback:function(e){t.qty=e},expression:"qty"}})],1),t._v(" "),s("div",{staticClass:"btn"},[s("Button",{attrs:{type:"success",long:""},on:{click:function(e){t.test3()}}},[t._v("提交验证test3")])],1)])]),t._v(" "),s("div",{staticClass:"object-model"},[s("div",{staticClass:"v4"},[s("h2",[t._v("实体校验指定字段：用户名、密码")]),t._v(" "),s("div",{staticClass:"v-input"},[t._m(5),t._v(" "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"输入用户名"},model:{value:t.loginInfo1.userName,callback:function(e){t.$set(t.loginInfo1,"userName",e)},expression:"loginInfo1.userName"}})],1),t._v(" "),s("div",{staticClass:"v-input"},[t._m(6),t._v(" "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"输入密码"},model:{value:t.loginInfo1.passWord,callback:function(e){t.$set(t.loginInfo1,"passWord",e)},expression:"loginInfo1.passWord"}})],1),t._v(" "),s("div",{staticClass:"v-input"},[s("label",[t._v("验证码：")]),t._v(" "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"输入验证码"},model:{value:t.loginInfo1.VerificationCode,callback:function(e){t.$set(t.loginInfo1,"VerificationCode",e)},expression:"loginInfo1.VerificationCode"}})],1),t._v(" "),s("div",{staticClass:"btn"},[s("Button",{attrs:{type:"success",long:""},on:{click:function(e){t.test4()}}},[t._v("提交验证test4")])],1)]),t._v(" "),s("div",{staticClass:"v5"},[s("h2",[t._v("实体校验指定字段：密码")]),t._v(" "),s("div",{staticClass:"v-input"},[s("label",[t._v("用户名：")]),t._v(" "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"输入用户名"},model:{value:t.loginInfo2.userName,callback:function(e){t.$set(t.loginInfo2,"userName",e)},expression:"loginInfo2.userName"}})],1),t._v(" "),s("div",{staticClass:"v-input"},[t._m(7),t._v(" "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"输入密码"},model:{value:t.loginInfo2.passWord,callback:function(e){t.$set(t.loginInfo2,"passWord",e)},expression:"loginInfo2.passWord"}})],1),t._v(" "),s("div",{staticClass:"v-input"},[s("label",[t._v("验证码：")]),t._v(" "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"输入验证码"},model:{value:t.loginInfo2.VerificationCode,callback:function(e){t.$set(t.loginInfo2,"VerificationCode",e)},expression:"loginInfo2.VerificationCode"}})],1),t._v(" "),s("div",{staticClass:"btn"},[s("Button",{attrs:{type:"success",long:""},on:{click:function(e){t.test5()}}},[t._v("提交验证test5")])],1)]),t._v(" "),s("div",{staticClass:"v6"},[s("h2",[t._v("实体字段：用户名、密码，普通参数")]),t._v(" "),s("div",{staticClass:"v-input"},[t._m(8),t._v(" "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"输入用户名"},model:{value:t.loginInfo3.userName,callback:function(e){t.$set(t.loginInfo3,"userName",e)},expression:"loginInfo3.userName"}})],1),t._v(" "),s("div",{staticClass:"v-input"},[t._m(9),t._v(" "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"输入密码"},model:{value:t.loginInfo3.passWord,callback:function(e){t.$set(t.loginInfo3,"passWord",e)},expression:"loginInfo3.passWord"}})],1),t._v(" "),s("div",{staticClass:"v-input"},[s("i",{staticClass:"require"},[t._v("*")]),t._v("手机号：\n          "),s("Input",{staticStyle:{width:"230px"},attrs:{placeholder:"输入手机号"},model:{value:t.phoneNo6,callback:function(e){t.phoneNo6=e},expression:"phoneNo6"}})],1),t._v(" "),s("div",{staticClass:"btn"},[s("Button",{attrs:{type:"success",long:""},on:{click:function(e){t.test6()}}},[t._v("提交验证test6")])],1)])])]),t._v(" "),s("br"),t._v(" "),s("br")],1)},staticRenderFns:[function(){var t=this.$createElement,e=this._self._c||t;return e("label",[e("i",{staticClass:"require"},[this._v("*")]),this._v("用户名：\n          ")])},function(){var t=this.$createElement,e=this._self._c||t;return e("label",[e("i",{staticClass:"require"},[this._v("*")]),this._v("手机号：\n          ")])},function(){var t=this.$createElement,e=this._self._c||t;return e("label",[e("i",{staticClass:"require"},[this._v("*")]),this._v("手机号：\n          ")])},function(){var t=this.$createElement,e=this._self._c||t;return e("label",[e("i",{staticClass:"require"},[this._v("*")]),this._v("所在地：\n          ")])},function(){var t=this.$createElement,e=this._self._c||t;return e("label",[e("i",{staticClass:"require"},[this._v("*")]),this._v("存货量：\n          ")])},function(){var t=this.$createElement,e=this._self._c||t;return e("label",[e("i",{staticClass:"require"},[this._v("*")]),this._v("用户名：\n          ")])},function(){var t=this.$createElement,e=this._self._c||t;return e("label",[e("i",{staticClass:"require"},[this._v("*")]),this._v("密 码：\n          ")])},function(){var t=this.$createElement,e=this._self._c||t;return e("label",[e("i",{staticClass:"require"},[this._v("*")]),this._v("密 码：\n          ")])},function(){var t=this.$createElement,e=this._self._c||t;return e("label",[e("i",{staticClass:"require"},[this._v("*")]),this._v("用户名：\n          ")])},function(){var t=this.$createElement,e=this._self._c||t;return e("label",[e("i",{staticClass:"require"},[this._v("*")]),this._v("密 码：\n          ")])}]};var a=s("VU/8")({data:function(){return{userName1:"",phoneNo1:"",userName2:"",phoneNo2:"",local:"",qty:"",phoneNo6:"",loginInfo1:{userName:"",passWord:"",VerificationCode:""},loginInfo2:{userName:"",passWord:"",VerificationCode:""},loginInfo3:{userName:"",passWord:""}}},methods:{test1:function(){var t=this,e="validatorExample/test1?userName="+this.userName1+"&phoneNo="+this.phoneNo1;this.http.post(e,{},"正在验证参数").then(function(e){t.$Message.info(e.message||e)})},test2:function(){var t=this,e="validatorExample/test2?userName="+this.userName1+"&phoneNo="+this.phoneNo1;this.http.post(e,{},"正在验证参数").then(function(e){t.$Message.info(e.message||e)})},test3:function(){var t=this,e="validatorExample/test3?local="+this.local+"&qty="+this.qty;this.http.post(e,{},"正在验证参数").then(function(e){t.$Message.info(e.message||e)})},test4:function(){var t=this;this.http.post("validatorExample/test4",this.loginInfo1,"正在验证参数").then(function(e){t.$Message.info(e.message||e)})},test5:function(){var t=this;this.http.post("validatorExample/test5",this.loginInfo2,"正在验证参数").then(function(e){t.$Message.info(e.message||e)})},test6:function(){var t=this,e="validatorExample/test6?phoneNo="+this.phoneNo6;this.http.post(e,this.loginInfo3,"正在验证参数").then(function(e){t.$Message.info(e.message||e)})}}},i,!1,function(t){s("TRBd")},"data-v-14c0266e",null);e.default=a.exports}});