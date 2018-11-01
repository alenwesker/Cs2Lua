require("cs2dsl__utility");
require("cs2dsl__namespaces");
require("cs2dsl__externenums");

class(Mandelbrot) {
	static_methods {
		__new_object = function(...){
			return(newobject(Mandelbrot, null, null, ...));
		};
		Test = function(){
			callinstance(newobject(Mandelbrot, "ctor", null), "Exec");
		};
		cctor = function(){
		};
	};
	static_fields {
	};
	static_props {};
	static_events {};

	instance_methods {
		Exec = function(this){
			local(width); width = 50;
			local(height); height = width;
			local(maxiter); maxiter = 50;
			local(limit); limit = 4.00;
			local(y); y = 0;
			while( execbinary("<", y, height, System.Int32, System.Int32, Struct, Struct, False, False) ){
				local(Ci); Ci = execbinary("-", execbinary("/", execbinary("*", 2.00, y, System.Double, System.Double, Struct, Struct, False, False), height, System.Double, System.Double, Struct, Struct, False, False), 1.00, System.Double, System.Double, Struct, Struct, False, False);
				local(x); x = 0;
				while( execbinary("<", x, width, System.Int32, System.Int32, Struct, Struct, False, False) ){
					local(Zr); Zr = 0.00;
					local(Zi); Zi = 0.00;
					local(Cr); Cr = execbinary("-", execbinary("/", execbinary("*", 2.00, x, System.Double, System.Double, Struct, Struct, False, False), width, System.Double, System.Double, Struct, Struct, False, False), 1.50, System.Double, System.Double, Struct, Struct, False, False);
					local(i); i = maxiter;
					local(isInside); isInside = true;
					do{
						local(Tr); Tr = execbinary("+", execbinary("-", execbinary("*", Zr, Zr, System.Double, System.Double, Struct, Struct, False, False), execbinary("*", Zi, Zi, System.Double, System.Double, Struct, Struct, False, False), System.Double, System.Double, Struct, Struct, False, False), Cr, System.Double, System.Double, Struct, Struct, False, False);
						Zi = execbinary("+", execbinary("*", execbinary("*", 2.00, Zr, System.Double, System.Double, Struct, Struct, False, False), Zi, System.Double, System.Double, Struct, Struct, False, False), Ci, System.Double, System.Double, Struct, Struct, False, False);
						Zr = Tr;
						if( execbinary(">", execbinary("+", execbinary("*", Zr, Zr, System.Double, System.Double, Struct, Struct, False, False), execbinary("*", Zi, Zi, System.Double, System.Double, Struct, Struct, False, False), System.Double, System.Double, Struct, Struct, False, False), limit, System.Double, System.Double, Struct, Struct, False, False) ){
							isInside = false;
							break;
						};
					}while(execbinary(">", (function(){ i = execbinary("-", i, 1, System.Int32, System.Int32, Struct, Struct, False, False); return(i); })(), 0, System.Int32, System.Int32, Struct, Struct, False, False));
					if( isInside ){
						callinstance(this, "Output", execbinary("/", execbinary("*", x, 1.00, System.Single, System.Single, Struct, Struct, False, False), width, System.Single, System.Single, Struct, Struct, False, False), execbinary("/", execbinary("*", y, 1.00, System.Single, System.Single, Struct, Struct, False, False), height, System.Single, System.Single, Struct, Struct, False, False));
					};
				x = execbinary("+", x, 1, System.Int32, System.Int32, Struct, Struct, False, False);
				};
			y = execbinary("+", y, 1, System.Int32, System.Int32, Struct, Struct, False, False);
			};
		};
		Output = function(this, x, y){
			callstatic(JsConsole, "Print", x, y);
		};
		ctor = function(this){
		};
	};
	instance_fields {
		r = 10;
		scale = 3.00;
		datas = arrayinit(1, 2, 3, 4, 5, 6);
		dicts = newexterndictionary(System.Collections.Generic.Dictionary_TKey_TValue, "System.Collections.Generic.Dictionary_TKey_TValue", "ctor", dictionaryinit(1 -> 1, 2 -> 2, 3 -> 3, 4 -> 4, 5 -> 5));
		dicts2 = newexterndictionary(System.Collections.Generic.Dictionary_TKey_TValue, "System.Collections.Generic.Dictionary_TKey_TValue", "ctor", dictionaryinit(), 128);
	};
	instance_props {};
	instance_events {};

	interfaces {};
	interface_map {};
};



