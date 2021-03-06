require("cs2lua__utility");
require("cs2lua__namespaces");
require("cs2lua__externenums");
require("cs2lua__interfaces");
require("luaconsole");
require("cusinghelper");

class(TestUnity, UnityEngine.MonoBehaviour) {
	static_methods {
		__new_object = function(...){
			return(newobject(TestUnity, typeargs(), typekinds(), null, null, ...));
		};
		cctor = function(){
		};
	};
	static_fields {
	};
	static_props {};
	static_events {};

	instance_methods {
		Test = function(this, ...){
			local{args = params(0);};
			if( execbinary(">=", getinstance(args, "Length"), 3, System.Int32, System.Int32, TypeKind.Struct, TypeKind.Struct) ){
				local(sagatObjId); sagatObjId = typecast(args[1], System.Int32, TypeKind.Struct);
				local(protectObjId); protectObjId = typecast(args[2], System.Int32, TypeKind.Struct);
				local(attackObjId); attackObjId = typecast(args[3], System.Int32, TypeKind.Struct);
			};
			local(t); t = callinstance(getinstance(this, "gameObject"), "GetComponent", UnityEngine.Transform);
			callinstance(getinstance(this, "gameObject"), "SetActive", true);
			local(r); r = callinstance(getinstance(this, "gameObject"), "GetComponent", UnityEngine.Renderer);
			setinstance(getinstance(this, "gameObject"), "active", true);
			local(v); v = true;
			local(s); s = invokeforbasicvalue(v, false, System.Boolean, "ToString");
			local(i); i = 123;
			local(s2); s2 = invokeforbasicvalue(i, false, System.Int32, "ToString");
			local(i2); i2 = invokeforbasicvalue(s2, false, System.String, "IndexOf", wrapchar('2', 0x032));
			callstatic(LuaConsole, "Print", i2);
			local(i3); i3 = getforbasicvalue(getinstance(this, "m_TestString"), false, System.String, "Length");
			local(c); c = getexterninstanceindexer(getinstance(this, "m_TestString"), null, "get_Chars", 2);
			local(equal); equal = execbinary("==", getinstance(this, "m_TestString"), s, System.String, System.String, TypeKind.Class, TypeKind.Class);
			local(a); a = buildarray(System.Int32, 5, 4, 3, 2, 1);
			local(ix); ix = invokearraystaticmethod(a, null, "IndexOf", System.Int32, a, 3);
			local(f); f = typecast(( (function(vv) {
				callstatic(LuaConsole, "Print", "test");
			};) ), System.Action_T, TypeKind.Delegate);
			f(123);
			local(isLoadingHeadIcon); isLoadingHeadIcon = false;
			local{__using_83_8_86_9 = newobject(CUsingHelper, typeargs(), typekinds(), "ctor", null, (function(){
				isLoadingHeadIcon = true;
			}), (function(){
				isLoadingHeadIcon = false;
			}));};
			callstatic(LuaConsole, "Print", "test");
			callinstance(__using_83_8_86_9, "Dispose");
			local(v1s); v1s = newexterndictionary(System.Collections.Generic.Dictionary_TKey_TValue, typeargs(System.Int32, System.Int32), typekinds(TypeKind.Struct, TypeKind.Struct), "System.Collections.Generic.Dictionary_TKey_TValue", "ctor", literaldictionary(1 => 2, 3 => 4, 5 => 6));
			local(v2s); v2s = newexterndictionary(System.Collections.Generic.Dictionary_TKey_TValue, typeargs(System.Int32, System.Int32), typekinds(TypeKind.Struct, TypeKind.Struct), "System.Collections.Generic.Dictionary_TKey_TValue", "ctor", literaldictionary(1 => 3, 5 => 4));
			local(v0); v0 = linq()from(function(){ return(v1s); })let(function(v1){ return(getinstance(v1, "Value")); })let(function(v1, v3){ return(invokeforbasicvalue(invokeforbasicvalue(v3, false, System.Int32, "ToString"), false, System.String, "Split", wrapchar(' ', 0x020))); })from(function(v1, v3, v4){ return(( linq()from(function(){ return(v2s); })select(function(v2){ return(v2); })end() )); })where(function(v1, v3, v4, vvvv){ return(execunary("!", callstatic(System.String, "IsNullOrEmpty", callinstance(v1, "ToString")), System.Boolean, TypeKind.Struct)); })select(function(v1, v3, v4, vvvv){ return(callinstance(v1, "ToString")); })end();
			local(v5); v5 = linq()from(function(){ return(v1s); })join((function(v1){ return(v2s); }), (function(v1, v2){ return(getinstance(v1, "Key")); }), (function(v1, v2){ return(getinstance(v2, "Key")); }))into()select(function(v1, ttt0){ return(anonymousobject{v1 = v1, v2 = callinstance(ttt0, "Count", System.Collections.Generic.KeyValuePair_TKey_TValue)}); })continuation()groupby((function(ttt){ return(ttt); }), (function(ttt){ return(getinstance(ttt, "v2")); }))end();
			foreach(vv, getiterator(v5)){
			};
		};
		ctor = function(this){
		};
	};
	instance_fields {
		m_TestString = "13579";
	};
	instance_props {};
	instance_events {};

	interfaces {};
	interface_map {};
};



