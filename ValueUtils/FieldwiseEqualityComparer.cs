using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ValueUtils {

    //TODO: unfinished: this code is currently fundamentally borked  It can't work, but it's not part of the public api, so I'm keeping it around to see if I can salvage something.

    //Fundamental problem: I can't compile an expression tree to an instance method.  No way whatsoever.

    static class FieldwiseEqualityComparer<T> {
        public static readonly IEqualityComparer<T> Instance = FieldwiseEqualityComparer.CreateComparer<T>();
    }

    static class FieldwiseEqualityComparer {
        static int idCounter = 0;

        static readonly AssemblyBuilder assemblyBuilder;
        static readonly ModuleBuilder moduleBuilder;
        static FieldwiseEqualityComparer() {
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ValueUtilsGeneratedAssembly"), AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule("ValueUtilsGeneratedModule");
        }

        public static IEqualityComparer<T> CreateComparer<T>() {
            var equalityFunc = FieldwiseEquality<T>.CreateLambda();
            var hashFunc = FieldwiseHasher<T>.CreateLambda();

            int id = Interlocked.Increment(ref idCounter);
            var typeBuilder = moduleBuilder.DefineType(typeof(T).Name + "_EqualityComparer_ValueUtilsGenerated" + id, TypeAttributes.Sealed, null, new[] { typeof(IEqualityComparer<T>) });
            typeBuilder.AddInterfaceImplementation(typeof(IEqualityComparer<T>));

            MethodBuilder equalityMethodBuilder =
             typeBuilder.DefineMethod("Equals",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(bool), new Type[] { typeof(T), typeof(T) });
            equalityFunc.CompileToMethod(equalityMethodBuilder);
            typeBuilder.DefineMethodOverride(equalityMethodBuilder,
                ((Func<T, T, bool>)default(IEqualityComparer<T>).Equals).Method
                );

            MethodBuilder hashMethodBuilder =
             typeBuilder.DefineMethod("GetHashCode",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(int), new Type[] { typeof(T) });
            hashFunc.CompileToMethod(hashMethodBuilder);

            typeBuilder.DefineMethodOverride(hashMethodBuilder,
                ((Func<T, int>)default(IEqualityComparer<T>).GetHashCode).Method
                );

            var type = typeBuilder.CreateType();
            return (IEqualityComparer<T>)Activator.CreateInstance(type);
        }
    }
}
