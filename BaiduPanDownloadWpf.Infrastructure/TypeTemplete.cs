using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace BaiduPanDownloadWpf.Infrastructure
{
    public sealed class TypeTemplate
    {
        public class ObjectContainer
        {
            private List<object> Container = new List<object>();
            private List<Type> TypeContainer = new List<Type>();

            public ObjectContainer(object[] objs, Type[] tlist)
            {
                Container.AddRange(objs);
                TypeContainer.AddRange(tlist);
            }

            public object Get(int index)
            {
                if (index >= 0 && index < Container.Count)
                    return Container[index];
                else
                    return null;
            }

            public Type GetType(int index)
            {
                if (index >= 0 && index < TypeContainer.Count)
                    return TypeContainer[index];
                else
                    return null;
            }

            public static MethodInfo GetGetMethodInfo()
            {
                MethodInfo[] mis = typeof(ObjectContainer).GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var mi in mis)
                {
                    if (mi.Name == "Get") return mi;
                }
                return null;
            }
        }

        private class MethodInfoHolder
        {
            public int ObjectIndex { get; private set; }
            public MethodInfo MethodInfo { get; private set; }
            public Type ObjectType { get; private set; }
            public MethodInfoHolder(MethodInfo pi, int index, Type type)
            {
                MethodInfo = pi;
                ObjectIndex = index;
                ObjectType = type;
            }
        }

        public delegate void Handler<TImple>(TImple imple) where TImple : class;

        public static TInterface Create<TInterface, TImple>(TImple instance)
            where TInterface : class
            where TImple : class
        {
            var type = DynamicTypeGen<TInterface>(new[] { instance }, new Type[] { instance.GetType() });
            return Activator.CreateInstance(type, new ObjectContainer(new[] { instance }, new Type[] { instance.GetType() })) as TInterface;
        }

        public static TInterface Create<TInterface>(params object[] impleInstances)
            where TInterface : class
        {
            var tlist = new List<Type>();
            foreach (var item in impleInstances)
            {
                tlist.Add(item.GetType());
            }
            var type = DynamicTypeGen<TInterface>(impleInstances, tlist.ToArray());
            return Activator.CreateInstance(type, new ObjectContainer(impleInstances, tlist.ToArray())) as TInterface;
        }

        public static TInterface CreateIntercepted<TInterface, TImple>(TImple instance, Handler<TImple> before, Handler<TImple> after)
            where TInterface : class
            where TImple : class
        {
            throw new NotImplementedException();
        }

        public static Type DynamicTypeGen<TInterface>(object[] instances, Type[] typeList)
            where TInterface : class
        {
            var tInterface = typeof(TInterface);

            var pisInterface = tInterface.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var misInterface = tInterface.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            var misInterfaceList = new List<MethodInfo>();

            foreach (var item in misInterface)
            {
                if (item.IsSpecialName == false) misInterfaceList.Add(item);
            }

            var miHolderList = new List<MethodInfoHolder>();
            for (int i = 0; i < typeList.Length; i++)
            {
                foreach (var item in typeList[i].GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    miHolderList.Add(new MethodInfoHolder(item, i, typeList[i]));
                }
            }

            var aName = new AssemblyName("Orc.Generics.DynamicTypes");
            var ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);

            var mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");

            var tb = mb.DefineType(GetDynamicTypeName<TInterface>(instances), TypeAttributes.Public, null, new Type[] { tInterface });
            var fbInstances = tb.DefineField("_container", typeof(TypeTemplate.ObjectContainer), FieldAttributes.Private);

            var ctor1 = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[1] { typeof(ObjectContainer) });

            var ctor1IL = ctor1.GetILGenerator();
            ctor1IL.Emit(OpCodes.Ldarg_0);
            ctor1IL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            ctor1IL.Emit(OpCodes.Ldarg_0);
            ctor1IL.Emit(OpCodes.Ldarg_1);
            ctor1IL.Emit(OpCodes.Stfld, fbInstances);
            ctor1IL.Emit(OpCodes.Ret);

            foreach (var item in pisInterface)
            {
                var getMi = miHolderList.FirstOrDefault(element => element.MethodInfo.Name.Equals("get_" + item.Name) && element.MethodInfo.IsSpecialName);
                var setMi = miHolderList.FirstOrDefault(element => element.MethodInfo.Name.Equals("set_" + item.Name) && element.MethodInfo.IsSpecialName);
                CreateProperty(tb, fbInstances, item, getMi, setMi);
            }
            foreach (var item in misInterfaceList)
            {
                var instanceMi = miHolderList.FirstOrDefault(element => MethodInfoEqual(element.MethodInfo, item));
                CreateMethod(tb, fbInstances, item, instanceMi);
            }
            var type = tb.CreateType();
            ab.Save(aName.Name + ".dll");
            return type;
        }

        private static bool MethodInfoEqual(MethodInfo mi1, MethodInfo mi2)
        {
            if (mi1.IsSpecialName == true || mi2.IsSpecialName == true) return false;
            if (mi1.Name != mi2.Name) return false;
            if (mi1.ReturnType != mi2.ReturnType) return false;
            var pis1 = mi1.GetParameters();
            var pis2 = mi2.GetParameters();
            if (pis1.Length != pis2.Length) return false;
            for (int i = 0; i < pis1.Length; i++)
            {
                if (pis1[i].ParameterType != pis2[i].ParameterType) return false;
            }
            return true;
        }

        private static void CreateProperty(TypeBuilder tb, FieldBuilder fbInstance, PropertyInfo pi, MethodInfoHolder getMi, MethodInfoHolder setMi)
        {
            var name = pi.Name;
            var type = pi.PropertyType;

            var pb = tb.DefineProperty(name, PropertyAttributes.HasDefault, type, null);

            var getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;
            var mbGetAccessor = tb.DefineMethod("get_" + name, getSetAttr, type, Type.EmptyTypes);

            var getIL = mbGetAccessor.GetILGenerator();

            if (getMi == null)
            {
                getIL.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructor(new Type[] { }));
                getIL.Emit(OpCodes.Throw);
            }
            else
            {
                getIL.Emit(OpCodes.Ldarg_0);
                getIL.Emit(OpCodes.Ldfld, fbInstance);
                getIL.Emit(OpCodes.Ldc_I4, getMi.ObjectIndex);
                getIL.Emit(OpCodes.Callvirt, ObjectContainer.GetGetMethodInfo());
                getIL.Emit(OpCodes.Isinst, getMi.ObjectType);
                getIL.Emit(OpCodes.Callvirt, getMi.MethodInfo);
                getIL.Emit(OpCodes.Ret);
            }

            var mbSetAccessor = tb.DefineMethod("set_" + name, getSetAttr, null, new Type[] { type });

            var setIL = mbSetAccessor.GetILGenerator();
            if (setMi == null)
            {
                setIL.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructor(new Type[] { }));
                setIL.Emit(OpCodes.Throw);
            }
            else
            {
                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Ldfld, fbInstance);
                setIL.Emit(OpCodes.Ldc_I4, setMi.ObjectIndex);
                setIL.Emit(OpCodes.Callvirt, ObjectContainer.GetGetMethodInfo());
                setIL.Emit(OpCodes.Isinst, setMi.ObjectType);
                setIL.Emit(OpCodes.Ldarg_1);
                setIL.Emit(OpCodes.Callvirt, setMi.MethodInfo);
                setIL.Emit(OpCodes.Ret);
            }

            pb.SetGetMethod(mbGetAccessor);
            pb.SetSetMethod(mbSetAccessor);
        }

        private static void CreateMethod(TypeBuilder tb, FieldBuilder fbInstance, MethodInfo mi, MethodInfoHolder instanceMi)
        {
            var paramTyleList = new List<Type>();
            foreach (var item in mi.GetParameters())
            {
                paramTyleList.Add(item.ParameterType);
            }

            var mb = tb.DefineMethod(mi.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final, mi.ReturnType, paramTyleList.ToArray());

            var il = mb.GetILGenerator();
            if (instanceMi == null)
            {
                il.Emit(OpCodes.Newobj, typeof(NotImplementedException).GetConstructor(new Type[] { }));
                il.Emit(OpCodes.Throw);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fbInstance);
                il.Emit(OpCodes.Ldc_I4, instanceMi.ObjectIndex);
                il.Emit(OpCodes.Callvirt, ObjectContainer.GetGetMethodInfo());
                il.Emit(OpCodes.Isinst, instanceMi.ObjectType);
                switch (paramTyleList.Count)
                {
                    case 0:
                        break;
                    case 1:
                        il.Emit(OpCodes.Ldarg_1);
                        break;
                    case 2:
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldarg_2);
                        break;
                    case 3:
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldarg_2);
                        il.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldarg_2);
                        il.Emit(OpCodes.Ldarg_3);

                        var sCount = Math.Min(paramTyleList.Count, 127);
                        for (int i = 4; i <= sCount; i++)
                        {
                            il.Emit(OpCodes.Ldarg_S, i);
                        }

                        for (int i = 128; i <= paramTyleList.Count; i++)
                        {
                            il.Emit(OpCodes.Ldarg, i);
                        }

                        break;
                }

                il.Emit(OpCodes.Callvirt, instanceMi.MethodInfo);
                il.Emit(OpCodes.Ret);
            }
        }

        private static string GetDynamicTypeName<TInterface>(params object[] instances) where TInterface : class
        {
            var sb = new StringBuilder();
            sb.Append("_DynamicTypes");
            sb.Append(typeof(TInterface).ToString());
            foreach (var obj in instances)
            {
                sb.Append("_");
                sb.Append(obj.GetType().ToString());
            }
            return sb.ToString();
        }
    }
}

