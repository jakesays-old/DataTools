using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Std.Utility.Reflection.Emit
{
	public static class ILGeneratorExtensions
	{
		/// <summary>
		/// Fills space if opcodes are patched. No meaningful operation is performed although
		/// a processing cycle can be consumed.
		/// </summary>
		public static ILGenerator nop(this ILGenerator il)
		{
			il.Emit(OpCodes.Nop);
			return il;
		}


		/// <summary>
		/// Signals the Common Language Infrastructure (CLI) to inform the debugger that a break
		/// point has been tripped.
		/// </summary>
		public static ILGenerator @break(this ILGenerator il)
		{
			il.Emit(OpCodes.Break);
			return il;
		}


		/// <summary>
		/// Loads the argument at index 0 onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldarg_0(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldarg_0);
			return il;
		}


		/// <summary>
		/// Loads the argument at index 1 onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldarg_1(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldarg_1);
			return il;
		}


		/// <summary>
		/// Loads the argument at index 2 onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldarg_2(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldarg_2);
			return il;
		}


		/// <summary>
		/// Loads the argument at index 3 onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldarg_3(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldarg_3);
			return il;
		}


		/// <summary>
		/// Loads the local variable at index 0 onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldloc_0(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldloc_0);
			return il;
		}


		/// <summary>
		/// Loads the local variable at index 1 onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldloc_1(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldloc_1);
			return il;
		}


		/// <summary>
		/// Loads the local variable at index 2 onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldloc_2(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldloc_2);
			return il;
		}


		/// <summary>
		/// Loads the local variable at index 3 onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldloc_3(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldloc_3);
			return il;
		}


		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in a the
		/// local variable list at index 0.
		/// </summary>
		public static ILGenerator stloc_0(this ILGenerator il)
		{
			il.Emit(OpCodes.Stloc_0);
			return il;
		}


		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in a the
		/// local variable list at index 1.
		/// </summary>
		public static ILGenerator stloc_1(this ILGenerator il)
		{
			il.Emit(OpCodes.Stloc_1);
			return il;
		}


		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in a the
		/// local variable list at index 2.
		/// </summary>
		public static ILGenerator stloc_2(this ILGenerator il)
		{
			il.Emit(OpCodes.Stloc_2);
			return il;
		}


		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in a the
		/// local variable list at index 3.
		/// </summary>
		public static ILGenerator stloc_3(this ILGenerator il)
		{
			il.Emit(OpCodes.Stloc_3);
			return il;
		}


		/// <summary>
		/// Loads the argument (referenced by a specified short form index) onto the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator ldarg_s(this ILGenerator il, byte arg1)
		{
			il.Emit(OpCodes.Ldarg_S, arg1);
			return il;
		}


		/// <summary>
		/// Load an argument address, in short form, onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldarga_s(this ILGenerator il, byte arg1)
		{
			il.Emit(OpCodes.Ldarga_S, arg1);
			return il;
		}


		/// <summary>
		/// Stores the value on top of the evaluation stack in the argument slot at a specified
		/// index, short form.
		/// </summary>
		public static ILGenerator starg_s(this ILGenerator il, byte arg1)
		{
			il.Emit(OpCodes.Starg_S, arg1);
			return il;
		}


		/// <summary>
		/// Loads the local variable at a specific index onto the evaluation stack, short form.
		/// </summary>
		public static ILGenerator ldloc_s(this ILGenerator il, LocalBuilder arg1)
		{
			il.Emit(OpCodes.Ldloc_S, arg1);
			return il;
		}


		/// <summary>
		/// Loads the local variable at a specific index onto the evaluation stack, short form.
		/// </summary>
		public static ILGenerator ldloc_s(this ILGenerator il, byte arg1)
		{
			il.Emit(OpCodes.Ldloc_S, arg1);
			return il;
		}


		/// <summary>
		/// Loads the address of the local variable at a specific index onto the evaluation stack,
		/// short form.
		/// </summary>
		public static ILGenerator ldloca_s(this ILGenerator il, byte arg1)
		{
			il.Emit(OpCodes.Ldloca_S, arg1);
			return il;
		}


		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in a the
		/// local variable list at index (short form).
		/// </summary>
		public static ILGenerator stloc_s(this ILGenerator il, LocalBuilder arg1)
		{
			il.Emit(OpCodes.Stloc_S, arg1);
			return il;
		}


		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in a the
		/// local variable list at index (short form).
		/// </summary>
		public static ILGenerator stloc_s(this ILGenerator il, byte arg1)
		{
			il.Emit(OpCodes.Stloc_S, arg1);
			return il;
		}


		/// <summary>
		/// Pushes a null reference (type O) onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldnull(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldnull);
			return il;
		}


		/// <summary>
		/// Pushes the integer value of -1 onto the evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldc_i4_m1(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldc_I4_M1);
			return il;
		}


		/// <summary>
		/// Pushes the integer value of 0 onto the evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldc_i4_0(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldc_I4_0);
			return il;
		}


		/// <summary>
		/// Pushes the integer value of 1 onto the evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldc_i4_1(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldc_I4_1);
			return il;
		}


		/// <summary>
		/// Pushes the integer value of 2 onto the evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldc_i4_2(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldc_I4_2);
			return il;
		}


		/// <summary>
		/// Pushes the integer value of 3 onto the evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldc_i4_3(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldc_I4_3);
			return il;
		}


		/// <summary>
		/// Pushes the integer value of 4 onto the evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldc_i4_4(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldc_I4_4);
			return il;
		}


		/// <summary>
		/// Pushes the integer value of 5 onto the evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldc_i4_5(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldc_I4_5);
			return il;
		}


		/// <summary>
		/// Pushes the integer value of 6 onto the evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldc_i4_6(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldc_I4_6);
			return il;
		}


		/// <summary>
		/// Pushes the integer value of 7 onto the evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldc_i4_7(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldc_I4_7);
			return il;
		}


		/// <summary>
		/// Pushes the integer value of 8 onto the evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldc_i4_8(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldc_I4_8);
			return il;
		}


		/// <summary>
		/// Pushes the supplied int8 value onto the evaluation stack as an int32, short form.
		/// </summary>
		public static ILGenerator ldc_i4_s(this ILGenerator il, byte arg1)
		{
			il.Emit(OpCodes.Ldc_I4_S, arg1);
			return il;
		}


		/// <summary>
		/// Pushes a supplied value of type int32 onto the evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldc_i4(this ILGenerator il, int arg1)
		{
			il.Emit(OpCodes.Ldc_I4, arg1);
			return il;
		}


		/// <summary>
		/// Pushes a supplied value of type int64 onto the evaluation stack as an int64.
		/// </summary>
		public static ILGenerator ldc_i8(this ILGenerator il, long arg1)
		{
			il.Emit(OpCodes.Ldc_I8, arg1);
			return il;
		}


		/// <summary>
		/// Pushes a supplied value of type float32 onto the evaluation stack as type F (float).
		/// </summary>
		public static ILGenerator ldc_r4(this ILGenerator il, float arg1)
		{
			il.Emit(OpCodes.Ldc_R4, arg1);
			return il;
		}


		/// <summary>
		/// Pushes a supplied value of type float64 onto the evaluation stack as type F (float).
		/// </summary>
		public static ILGenerator ldc_r8(this ILGenerator il, double arg1)
		{
			il.Emit(OpCodes.Ldc_R8, arg1);
			return il;
		}


		/// <summary>
		/// Copies the current topmost value on the evaluation stack, and then pushes the copy
		/// onto the evaluation stack.
		/// </summary>
		public static ILGenerator dup(this ILGenerator il)
		{
			il.Emit(OpCodes.Dup);
			return il;
		}


		/// <summary>
		/// Removes the value currently on top of the evaluation stack.
		/// </summary>
		public static ILGenerator pop(this ILGenerator il)
		{
			il.Emit(OpCodes.Pop);
			return il;
		}


		/// <summary>
		/// Exits current method and jumps to specified method.
		/// </summary>
		public static ILGenerator jmp(this ILGenerator il, MethodInfo arg1)
		{
			il.Emit(OpCodes.Jmp, arg1);
			return il;
		}


		/// <summary>
		/// Calls the method indicated by the passed method descriptor.
		/// </summary>
		public static ILGenerator call(this ILGenerator il, MethodInfo arg1)
		{
			il.Emit(OpCodes.Call, arg1);
			return il;
		}


		/// <summary>
		/// Calls the method indicated by the passed method descriptor.
		/// </summary>
		public static ILGenerator call(this ILGenerator il, MethodInfo arg1, Type[] arg2)
		{
			il.EmitCall(OpCodes.Call, arg1, arg2);
			return il;
		}


		/// <summary>
		/// Calls the method indicated on the evaluation stack (as a pointer to an entry point)
		/// with arguments described by a calling convention.
		/// </summary>
		public static ILGenerator calli(this ILGenerator il, CallingConventions arg1, Type arg2, Type[] arg3, Type[] arg4)
		{
			il.EmitCalli(OpCodes.Calli, arg1, arg2, arg3, arg4);
			return il;
		}


		/// <summary>
		/// Calls the method indicated on the evaluation stack (as a pointer to an entry point)
		/// with arguments described by a calling convention.
		/// </summary>
		public static ILGenerator calli(this ILGenerator il, CallingConvention arg1, Type arg2, Type[] arg3)
		{
			il.EmitCalli(OpCodes.Calli, arg1, arg2, arg3);
			return il;
		}


		/// <summary>
		/// Returns from the current method, pushing a return value (if present) from the callee's
		/// evaluation stack onto the caller's evaluation stack.
		/// </summary>
		public static ILGenerator ret(this ILGenerator il)
		{
			il.Emit(OpCodes.Ret);
			return il;
		}


		/// <summary>
		/// Unconditionally transfers control to a target instruction (short form).
		/// </summary>
		public static ILGenerator br_s(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Br_S, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction if value is false, a null reference, or
		/// zero.
		/// </summary>
		public static ILGenerator brfalse_s(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Brfalse_S, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction (short form) if value is true, not null,
		/// or non-zero.
		/// </summary>
		public static ILGenerator brtrue_s(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Brtrue_S, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction (short form) if two values are equal.
		/// </summary>
		public static ILGenerator beq_s(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Beq_S, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is greater
		/// than or equal to the second value.
		/// </summary>
		public static ILGenerator bge_s(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Bge_S, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is greater
		/// than the second value.
		/// </summary>
		public static ILGenerator bgt_s(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Bgt_S, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is less
		/// than or equal to the second value.
		/// </summary>
		public static ILGenerator ble_s(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Ble_S, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is less
		/// than the second value.
		/// </summary>
		public static ILGenerator blt_s(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Blt_S, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction (short form) when two unsigned integer
		/// values or unordered float values are not equal.
		/// </summary>
		public static ILGenerator bne_un_s(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Bne_Un_S, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is greater
		/// than the second value, when comparing unsigned integer values or unordered float
		/// values.
		/// </summary>
		public static ILGenerator bge_un_s(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Bge_Un_S, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is greater
		/// than the second value, when comparing unsigned integer values or unordered float
		/// values.
		/// </summary>
		public static ILGenerator bgt_un_s(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Bgt_Un_S, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is less
		/// than or equal to the second value, when comparing unsigned integer values or unordered
		/// float values.
		/// </summary>
		public static ILGenerator ble_un_s(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Ble_Un_S, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction (short form) if the first value is less
		/// than the second value, when comparing unsigned integer values or unordered float
		/// values.
		/// </summary>
		public static ILGenerator blt_un_s(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Blt_Un_S, arg1);
			return il;
		}


		/// <summary>
		/// Unconditionally transfers control to a target instruction.
		/// </summary>
		public static ILGenerator br(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Br, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction if value is false, a null reference (Nothing
		/// in Visual Basic), or zero.
		/// </summary>
		public static ILGenerator brfalse(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Brfalse, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction if value is true, not null, or non-zero.
		/// </summary>
		public static ILGenerator brtrue(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Brtrue, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction if two values are equal.
		/// </summary>
		public static ILGenerator beq(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Beq, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction if the first value is greater than or equal
		/// to the second value.
		/// </summary>
		public static ILGenerator bge(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Bge, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction if the first value is greater than the
		/// second value.
		/// </summary>
		public static ILGenerator bgt(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Bgt, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction if the first value is less than or equal
		/// to the second value.
		/// </summary>
		public static ILGenerator ble(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Ble, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction if the first value is less than the second
		/// value.
		/// </summary>
		public static ILGenerator blt(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Blt, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction when two unsigned integer values or unordered
		/// float values are not equal.
		/// </summary>
		public static ILGenerator bne_un(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Bne_Un, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction if the first value is greater than the
		/// second value, when comparing unsigned integer values or unordered float values.
		/// </summary>
		public static ILGenerator bge_un(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Bge_Un, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction if the first value is greater than the
		/// second value, when comparing unsigned integer values or unordered float values.
		/// </summary>
		public static ILGenerator bgt_un(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Bgt_Un, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction if the first value is less than or equal
		/// to the second value, when comparing unsigned integer values or unordered float values.
		/// </summary>
		public static ILGenerator ble_un(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Ble_Un, arg1);
			return il;
		}


		/// <summary>
		/// Transfers control to a target instruction if the first value is less than the second
		/// value, when comparing unsigned integer values or unordered float values.
		/// </summary>
		public static ILGenerator blt_un(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Blt_Un, arg1);
			return il;
		}


		/// <summary>
		/// Implements a jump table.
		/// </summary>
		public static ILGenerator switch_(this ILGenerator il, Label[] arg1)
		{
			il.Emit(OpCodes.Switch, arg1);
			return il;
		}


		/// <summary>
		/// Loads a value of type int8 as an int32 onto the evaluation stack indirectly.
		/// </summary>
		public static ILGenerator ldind_i1(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldind_I1);
			return il;
		}


		/// <summary>
		/// Loads a value of type unsigned int8 as an int32 onto the evaluation stack indirectly.
		/// </summary>
		public static ILGenerator ldind_u1(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldind_U1);
			return il;
		}


		/// <summary>
		/// Loads a value of type int16 as an int32 onto the evaluation stack indirectly.
		/// </summary>
		public static ILGenerator ldind_i2(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldind_I2);
			return il;
		}


		/// <summary>
		/// Loads a value of type unsigned int16 as an int32 onto the evaluation stack indirectly.
		/// </summary>
		public static ILGenerator ldind_u2(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldind_U2);
			return il;
		}


		/// <summary>
		/// Loads a value of type int32 as an int32 onto the evaluation stack indirectly.
		/// </summary>
		public static ILGenerator ldind_i4(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldind_I4);
			return il;
		}


		/// <summary>
		/// Loads a value of type unsigned int32 as an int32 onto the evaluation stack indirectly.
		/// </summary>
		public static ILGenerator ldind_u4(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldind_U4);
			return il;
		}


		/// <summary>
		/// Loads a value of type int64 as an int64 onto the evaluation stack indirectly.
		/// </summary>
		public static ILGenerator ldind_i8(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldind_I8);
			return il;
		}


		/// <summary>
		/// Loads a value of type native int as a native int onto the evaluation stack indirectly.
		/// </summary>
		public static ILGenerator ldind_i(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldind_I);
			return il;
		}


		/// <summary>
		/// Loads a value of type float32 as a type F (float) onto the evaluation stack indirectly.
		/// </summary>
		public static ILGenerator ldind_r4(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldind_R4);
			return il;
		}


		/// <summary>
		/// Loads a value of type float64 as a type F (float) onto the evaluation stack indirectly.
		/// </summary>
		public static ILGenerator ldind_r8(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldind_R8);
			return il;
		}


		/// <summary>
		/// Loads an object reference as a type O (object reference) onto the evaluation stack
		/// indirectly.
		/// </summary>
		public static ILGenerator ldind_ref(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldind_Ref);
			return il;
		}


		/// <summary>
		/// Stores a object reference value at a supplied address.
		/// </summary>
		public static ILGenerator stind_ref(this ILGenerator il)
		{
			il.Emit(OpCodes.Stind_Ref);
			return il;
		}


		/// <summary>
		/// Stores a value of type int8 at a supplied address.
		/// </summary>
		public static ILGenerator stind_i1(this ILGenerator il)
		{
			il.Emit(OpCodes.Stind_I1);
			return il;
		}


		/// <summary>
		/// Stores a value of type int16 at a supplied address.
		/// </summary>
		public static ILGenerator stind_i2(this ILGenerator il)
		{
			il.Emit(OpCodes.Stind_I2);
			return il;
		}


		/// <summary>
		/// Stores a value of type int32 at a supplied address.
		/// </summary>
		public static ILGenerator stind_i4(this ILGenerator il)
		{
			il.Emit(OpCodes.Stind_I4);
			return il;
		}


		/// <summary>
		/// Stores a value of type int64 at a supplied address.
		/// </summary>
		public static ILGenerator stind_i8(this ILGenerator il)
		{
			il.Emit(OpCodes.Stind_I8);
			return il;
		}


		/// <summary>
		/// Stores a value of type float32 at a supplied address.
		/// </summary>
		public static ILGenerator stind_r4(this ILGenerator il)
		{
			il.Emit(OpCodes.Stind_R4);
			return il;
		}


		/// <summary>
		/// Stores a value of type float64 at a supplied address.
		/// </summary>
		public static ILGenerator stind_r8(this ILGenerator il)
		{
			il.Emit(OpCodes.Stind_R8);
			return il;
		}


		/// <summary>
		/// Adds two values and pushes the result onto the evaluation stack.
		/// </summary>
		public static ILGenerator add(this ILGenerator il)
		{
			il.Emit(OpCodes.Add);
			return il;
		}


		/// <summary>
		/// Subtracts one value from another and pushes the result onto the evaluation stack.
		/// </summary>
		public static ILGenerator sub(this ILGenerator il)
		{
			il.Emit(OpCodes.Sub);
			return il;
		}


		/// <summary>
		/// Multiplies two values and pushes the result on the evaluation stack.
		/// </summary>
		public static ILGenerator mul(this ILGenerator il)
		{
			il.Emit(OpCodes.Mul);
			return il;
		}


		/// <summary>
		/// Divides two values and pushes the result as a floating-point (type F) or quotient
		/// (type int32) onto the evaluation stack.
		/// </summary>
		public static ILGenerator div(this ILGenerator il)
		{
			il.Emit(OpCodes.Div);
			return il;
		}


		/// <summary>
		/// Divides two unsigned integer values and pushes the result (int32) onto the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator div_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Div_Un);
			return il;
		}


		/// <summary>
		/// Divides two values and pushes the remainder onto the evaluation stack.
		/// </summary>
		public static ILGenerator rem(this ILGenerator il)
		{
			il.Emit(OpCodes.Rem);
			return il;
		}


		/// <summary>
		/// Divides two unsigned values and pushes the remainder onto the evaluation stack.
		/// </summary>
		public static ILGenerator rem_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Rem_Un);
			return il;
		}


		/// <summary>
		/// Computes the bitwise AND of two values and pushes the result onto the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator and(this ILGenerator il)
		{
			il.Emit(OpCodes.And);
			return il;
		}


		/// <summary>
		/// Compute the bitwise complement of the two integer values on top of the stack and
		/// pushes the result onto the evaluation stack.
		/// </summary>
		public static ILGenerator or(this ILGenerator il)
		{
			il.Emit(OpCodes.Or);
			return il;
		}


		/// <summary>
		/// Computes the bitwise XOR of the top two values on the evaluation stack, pushing the
		/// result onto the evaluation stack.
		/// </summary>
		public static ILGenerator xor(this ILGenerator il)
		{
			il.Emit(OpCodes.Xor);
			return il;
		}


		/// <summary>
		/// Shifts an integer value to the left (in zeroes) by a specified number of bits, pushing
		/// the result onto the evaluation stack.
		/// </summary>
		public static ILGenerator shl(this ILGenerator il)
		{
			il.Emit(OpCodes.Shl);
			return il;
		}


		/// <summary>
		/// Shifts an integer value (in sign) to the right by a specified number of bits, pushing
		/// the result onto the evaluation stack.
		/// </summary>
		public static ILGenerator shr(this ILGenerator il)
		{
			il.Emit(OpCodes.Shr);
			return il;
		}


		/// <summary>
		/// Shifts an unsigned integer value (in zeroes) to the right by a specified number of
		/// bits, pushing the result onto the evaluation stack.
		/// </summary>
		public static ILGenerator shr_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Shr_Un);
			return il;
		}


		/// <summary>
		/// Negates a value and pushes the result onto the evaluation stack.
		/// </summary>
		public static ILGenerator neg(this ILGenerator il)
		{
			il.Emit(OpCodes.Neg);
			return il;
		}


		/// <summary>
		/// Computes the bitwise complement of the integer value on top of the stack and pushes
		/// the result onto the evaluation stack as the same type.
		/// </summary>
		public static ILGenerator not(this ILGenerator il)
		{
			il.Emit(OpCodes.Not);
			return il;
		}


		/// <summary>
		/// Converts the value on top of the evaluation stack to int8, then extends (pads) it
		/// to int32.
		/// </summary>
		public static ILGenerator conv_i1(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_I1);
			return il;
		}


		/// <summary>
		/// Converts the value on top of the evaluation stack to int16, then extends (pads) it
		/// to int32.
		/// </summary>
		public static ILGenerator conv_i2(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_I2);
			return il;
		}


		/// <summary>
		/// Converts the value on top of the evaluation stack to int32.
		/// </summary>
		public static ILGenerator conv_i4(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_I4);
			return il;
		}


		/// <summary>
		/// Converts the value on top of the evaluation stack to int64.
		/// </summary>
		public static ILGenerator conv_i8(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_I8);
			return il;
		}


		/// <summary>
		/// Converts the value on top of the evaluation stack to float32.
		/// </summary>
		public static ILGenerator conv_r4(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_R4);
			return il;
		}


		/// <summary>
		/// Converts the value on top of the evaluation stack to float64.
		/// </summary>
		public static ILGenerator conv_r8(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_R8);
			return il;
		}


		/// <summary>
		/// Converts the value on top of the evaluation stack to unsigned int32, and extends
		/// it to int32.
		/// </summary>
		public static ILGenerator conv_u4(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_U4);
			return il;
		}


		/// <summary>
		/// Converts the value on top of the evaluation stack to unsigned int64, and extends
		/// it to int64.
		/// </summary>
		public static ILGenerator conv_u8(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_U8);
			return il;
		}


		/// <summary>
		/// Calls a late-bound method on an object, pushing the return value onto the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator callvirt(this ILGenerator il, MethodInfo arg1)
		{
			il.Emit(OpCodes.Callvirt, arg1);
			return il;
		}


		/// <summary>
		/// Calls a late-bound method on an object, pushing the return value onto the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator callvirt(this ILGenerator il, MethodInfo arg1, Type[] arg2)
		{
			il.EmitCall(OpCodes.Callvirt, arg1, arg2);
			return il;
		}


		/// <summary>
		/// Copies the value type located at the address of an object (type &amp;, * or native
		/// int) to the address of the destination object (type &amp;, * or native int).
		/// </summary>
		public static ILGenerator cpobj(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Cpobj, arg1);
			return il;
		}


		/// <summary>
		/// Copies the value type object pointed to by an address to the top of the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator ldobj(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Ldobj, arg1);
			return il;
		}


		/// <summary>
		/// Pushes a new object reference to a string literal stored in the metadata.
		/// </summary>
		public static ILGenerator ldstr(this ILGenerator il, string arg1)
		{
			il.Emit(OpCodes.Ldstr, arg1);
			return il;
		}


		/// <summary>
		/// Creates a new object or a new instance of a value type, pushing an object reference
		/// (type O) onto the evaluation stack.
		/// </summary>
		public static ILGenerator newobj(this ILGenerator il, ConstructorInfo arg1)
		{
			il.Emit(OpCodes.Newobj, arg1);
			return il;
		}


		/// <summary>
		/// Attempts to cast an object passed by reference to the specified class.
		/// </summary>
		public static ILGenerator castclass(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Castclass, arg1);
			return il;
		}


		/// <summary>
		/// Tests whether an object reference (type O) is an instance of a particular class.
		/// </summary>
		public static ILGenerator isinst(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Isinst, arg1);
			return il;
		}


		/// <summary>
		/// Converts the unsigned integer value on top of the evaluation stack to float32.
		/// </summary>
		public static ILGenerator conv_r_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_R_Un);
			return il;
		}


		/// <summary>
		/// Converts the boxed representation of a value type to its unboxed form.
		/// </summary>
		public static ILGenerator unbox(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Unbox, arg1);
			return il;
		}


		/// <summary>
		/// Throws the exception object currently on the evaluation stack.
		/// </summary>
		public static ILGenerator throw_(this ILGenerator il)
		{
			il.Emit(OpCodes.Throw);
			return il;
		}


		/// <summary>
		/// Finds the value of a field in the object whose reference is currently on the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator ldfld(this ILGenerator il, FieldInfo arg1)
		{
			il.Emit(OpCodes.Ldfld, arg1);
			return il;
		}


		/// <summary>
		/// Finds the address of a field in the object whose reference is currently on the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator ldflda(this ILGenerator il, FieldInfo arg1)
		{
			il.Emit(OpCodes.Ldflda, arg1);
			return il;
		}


		/// <summary>
		/// Replaces the value stored in the field of an object reference or pointer with a new
		/// value.
		/// </summary>
		public static ILGenerator stfld(this ILGenerator il, FieldInfo arg1)
		{
			il.Emit(OpCodes.Stfld, arg1);
			return il;
		}


		/// <summary>
		/// Pushes the value of a static field onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldsfld(this ILGenerator il, FieldInfo arg1)
		{
			il.Emit(OpCodes.Ldsfld, arg1);
			return il;
		}


		/// <summary>
		/// Pushes the address of a static field onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldsflda(this ILGenerator il, FieldInfo arg1)
		{
			il.Emit(OpCodes.Ldsflda, arg1);
			return il;
		}


		/// <summary>
		/// Replaces the value of a static field with a value from the evaluation stack.
		/// </summary>
		public static ILGenerator stsfld(this ILGenerator il, FieldInfo arg1)
		{
			il.Emit(OpCodes.Stsfld, arg1);
			return il;
		}


		/// <summary>
		/// Copies a value of a specified type from the evaluation stack into a supplied memory
		/// address.
		/// </summary>
		public static ILGenerator stobj(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Stobj, arg1);
			return il;
		}


		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to signed int8 and extends
		/// it to int32, throwing OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_i1_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_I1_Un);
			return il;
		}


		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to signed int16 and extends
		/// it to int32, throwing OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_i2_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_I2_Un);
			return il;
		}


		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to signed int32, throwing
		/// OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_i4_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_I4_Un);
			return il;
		}


		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to signed int64, throwing
		/// OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_i8_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_I8_Un);
			return il;
		}


		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to unsigned int8 and extends
		/// it to int32, throwing OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_u1_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_U1_Un);
			return il;
		}


		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to unsigned int16 and
		/// extends it to int32, throwing OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_u2_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_U2_Un);
			return il;
		}


		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to unsigned int32, throwing
		/// OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_u4_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_U4_Un);
			return il;
		}


		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to unsigned int64, throwing
		/// OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_u8_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_U8_Un);
			return il;
		}


		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to signed native int,
		/// throwing OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_i_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_I_Un);
			return il;
		}


		/// <summary>
		/// Converts the unsigned value on top of the evaluation stack to unsigned native int,
		/// throwing OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_u_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_U_Un);
			return il;
		}


		/// <summary>
		/// Converts a value type to an object reference (type O).
		/// </summary>
		public static ILGenerator box(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Box, arg1);
			return il;
		}


		/// <summary>
		/// Pushes an object reference to a new zero-based, one-dimensional array whose elements
		/// are of a specific type onto the evaluation stack.
		/// </summary>
		public static ILGenerator newarr(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Newarr, arg1);
			return il;
		}


		/// <summary>
		/// Pushes the number of elements of a zero-based, one-dimensional array onto the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator ldlen(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldlen);
			return il;
		}


		/// <summary>
		/// Loads the address of the array element at a specified array index onto the top of
		/// the evaluation stack as type &amp; (managed pointer).
		/// </summary>
		public static ILGenerator ldelema(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Ldelema, arg1);
			return il;
		}


		/// <summary>
		/// Loads the element with type int8 at a specified array index onto the top of the evaluation
		/// stack as an int32.
		/// </summary>
		public static ILGenerator ldelem_i1(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldelem_I1);
			return il;
		}


		/// <summary>
		/// Loads the element with type unsigned int8 at a specified array index onto the top
		/// of the evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldelem_u1(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldelem_U1);
			return il;
		}


		/// <summary>
		/// Loads the element with type int16 at a specified array index onto the top of the
		/// evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldelem_i2(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldelem_I2);
			return il;
		}


		/// <summary>
		/// Loads the element with type unsigned int16 at a specified array index onto the top
		/// of the evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldelem_u2(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldelem_U2);
			return il;
		}


		/// <summary>
		/// Loads the element with type int32 at a specified array index onto the top of the
		/// evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldelem_i4(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldelem_I4);
			return il;
		}


		/// <summary>
		/// Loads the element with type unsigned int32 at a specified array index onto the top
		/// of the evaluation stack as an int32.
		/// </summary>
		public static ILGenerator ldelem_u4(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldelem_U4);
			return il;
		}


		/// <summary>
		/// Loads the element with type int64 at a specified array index onto the top of the
		/// evaluation stack as an int64.
		/// </summary>
		public static ILGenerator ldelem_i8(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldelem_I8);
			return il;
		}


		/// <summary>
		/// Loads the element with type native int at a specified array index onto the top of
		/// the evaluation stack as a native int.
		/// </summary>
		public static ILGenerator ldelem_i(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldelem_I);
			return il;
		}


		/// <summary>
		/// Loads the element with type float32 at a specified array index onto the top of the
		/// evaluation stack as type F (float).
		/// </summary>
		public static ILGenerator ldelem_r4(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldelem_R4);
			return il;
		}


		/// <summary>
		/// Loads the element with type float64 at a specified array index onto the top of the
		/// evaluation stack as type F (float).
		/// </summary>
		public static ILGenerator ldelem_r8(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldelem_R8);
			return il;
		}


		/// <summary>
		/// Loads the element containing an object reference at a specified array index onto
		/// the top of the evaluation stack as type O (object reference).
		/// </summary>
		public static ILGenerator ldelem_ref(this ILGenerator il)
		{
			il.Emit(OpCodes.Ldelem_Ref);
			return il;
		}


		/// <summary>
		/// Replaces the array element at a given index with the native int value on the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator stelem_i(this ILGenerator il)
		{
			il.Emit(OpCodes.Stelem_I);
			return il;
		}


		/// <summary>
		/// Replaces the array element at a given index with the int8 value on the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator stelem_i1(this ILGenerator il)
		{
			il.Emit(OpCodes.Stelem_I1);
			return il;
		}


		/// <summary>
		/// Replaces the array element at a given index with the int16 value on the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator stelem_i2(this ILGenerator il)
		{
			il.Emit(OpCodes.Stelem_I2);
			return il;
		}


		/// <summary>
		/// Replaces the array element at a given index with the int32 value on the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator stelem_i4(this ILGenerator il)
		{
			il.Emit(OpCodes.Stelem_I4);
			return il;
		}


		/// <summary>
		/// Replaces the array element at a given index with the int64 value on the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator stelem_i8(this ILGenerator il)
		{
			il.Emit(OpCodes.Stelem_I8);
			return il;
		}


		/// <summary>
		/// Replaces the array element at a given index with the float32 value on the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator stelem_r4(this ILGenerator il)
		{
			il.Emit(OpCodes.Stelem_R4);
			return il;
		}


		/// <summary>
		/// Replaces the array element at a given index with the float64 value on the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator stelem_r8(this ILGenerator il)
		{
			il.Emit(OpCodes.Stelem_R8);
			return il;
		}


		/// <summary>
		/// Replaces the array element at a given index with the object ref value (type O) on
		/// the evaluation stack.
		/// </summary>
		public static ILGenerator stelem_ref(this ILGenerator il)
		{
			il.Emit(OpCodes.Stelem_Ref);
			return il;
		}


		/// <summary>
		/// Loads the element at a specified array index onto the top of the evaluation stack
		/// as the type specified in the instruction.
		/// </summary>
		public static ILGenerator ldelem(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Ldelem, arg1);
			return il;
		}


		/// <summary>
		/// Replaces the array element at a given index with the value on the evaluation stack,
		/// whose type is specified in the instruction.
		/// </summary>
		public static ILGenerator stelem(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Stelem, arg1);
			return il;
		}


		/// <summary>
		/// Converts the boxed representation of a type specified in the instruction to its unboxed
		/// form.
		/// </summary>
		public static ILGenerator unbox_any(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Unbox_Any, arg1);
			return il;
		}


		/// <summary>
		/// Converts the signed value on top of the evaluation stack to signed int8 and extends
		/// it to int32, throwing OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_i1(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_I1);
			return il;
		}


		/// <summary>
		/// Converts the signed value on top of the evaluation stack to unsigned int8 and extends
		/// it to int32, throwing OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_u1(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_U1);
			return il;
		}


		/// <summary>
		/// Converts the signed value on top of the evaluation stack to signed int16 and extending
		/// it to int32, throwing OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_i2(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_I2);
			return il;
		}


		/// <summary>
		/// Converts the signed value on top of the evaluation stack to unsigned int16 and extends
		/// it to int32, throwing OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_u2(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_U2);
			return il;
		}


		/// <summary>
		/// Converts the signed value on top of the evaluation stack to signed int32, throwing
		/// OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_i4(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_I4);
			return il;
		}


		/// <summary>
		/// Converts the signed value on top of the evaluation stack to unsigned int32, throwing
		/// OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_u4(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_U4);
			return il;
		}


		/// <summary>
		/// Converts the signed value on top of the evaluation stack to signed int64, throwing
		/// OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_i8(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_I8);
			return il;
		}


		/// <summary>
		/// Converts the signed value on top of the evaluation stack to unsigned int64, throwing
		/// OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_u8(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_U8);
			return il;
		}


		/// <summary>
		/// Retrieves the address (type &amp;) embedded in a typed reference.
		/// </summary>
		public static ILGenerator refanyval(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Refanyval, arg1);
			return il;
		}


		/// <summary>
		/// Throws ArithmeticException if value is not a finite number.
		/// </summary>
		public static ILGenerator ckfinite(this ILGenerator il)
		{
			il.Emit(OpCodes.Ckfinite);
			return il;
		}


		/// <summary>
		/// Pushes a typed reference to an instance of a specific type onto the evaluation stack.
		/// </summary>
		public static ILGenerator mkrefany(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Mkrefany, arg1);
			return il;
		}


		/// <summary>
		/// Converts a metadata token to its runtime representation, pushing it onto the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator ldtoken(this ILGenerator il, MethodInfo arg1)
		{
			il.Emit(OpCodes.Ldtoken, arg1);
			return il;
		}


		/// <summary>
		/// Converts a metadata token to its runtime representation, pushing it onto the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator ldtoken(this ILGenerator il, FieldInfo arg1)
		{
			il.Emit(OpCodes.Ldtoken, arg1);
			return il;
		}


		/// <summary>
		/// Converts a metadata token to its runtime representation, pushing it onto the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator ldtoken(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Ldtoken, arg1);
			return il;
		}


		/// <summary>
		/// Converts the value on top of the evaluation stack to unsigned int16, and extends
		/// it to int32.
		/// </summary>
		public static ILGenerator conv_u2(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_U2);
			return il;
		}


		/// <summary>
		/// Converts the value on top of the evaluation stack to unsigned int8, and extends it
		/// to int32.
		/// </summary>
		public static ILGenerator conv_u1(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_U1);
			return il;
		}


		/// <summary>
		/// Converts the value on top of the evaluation stack to native int.
		/// </summary>
		public static ILGenerator conv_i(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_I);
			return il;
		}


		/// <summary>
		/// Converts the signed value on top of the evaluation stack to signed native int, throwing
		/// OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_i(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_I);
			return il;
		}


		/// <summary>
		/// Converts the signed value on top of the evaluation stack to unsigned native int,
		/// throwing OverflowException on overflow.
		/// </summary>
		public static ILGenerator conv_ovf_u(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_Ovf_U);
			return il;
		}


		/// <summary>
		/// Adds two integers, performs an overflow check, and pushes the result onto the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator add_ovf(this ILGenerator il)
		{
			il.Emit(OpCodes.Add_Ovf);
			return il;
		}


		/// <summary>
		/// Adds two unsigned integer values, performs an overflow check, and pushes the result
		/// onto the evaluation stack.
		/// </summary>
		public static ILGenerator add_ovf_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Add_Ovf_Un);
			return il;
		}


		/// <summary>
		/// Multiplies two integer values, performs an overflow check, and pushes the result
		/// onto the evaluation stack.
		/// </summary>
		public static ILGenerator mul_ovf(this ILGenerator il)
		{
			il.Emit(OpCodes.Mul_Ovf);
			return il;
		}


		/// <summary>
		/// Multiplies two unsigned integer values, performs an overflow check, and pushes the
		/// result onto the evaluation stack.
		/// </summary>
		public static ILGenerator mul_ovf_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Mul_Ovf_Un);
			return il;
		}


		/// <summary>
		/// Subtracts one integer value from another, performs an overflow check, and pushes
		/// the result onto the evaluation stack.
		/// </summary>
		public static ILGenerator sub_ovf(this ILGenerator il)
		{
			il.Emit(OpCodes.Sub_Ovf);
			return il;
		}


		/// <summary>
		/// Subtracts one unsigned integer value from another, performs an overflow check, and
		/// pushes the result onto the evaluation stack.
		/// </summary>
		public static ILGenerator sub_ovf_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Sub_Ovf_Un);
			return il;
		}


		/// <summary>
		/// Transfers control from the fault or finally clause of an exception block back to
		/// the Common Language Infrastructure (CLI) exception handler.
		/// </summary>
		public static ILGenerator endfinally(this ILGenerator il)
		{
			il.Emit(OpCodes.Endfinally);
			return il;
		}


		/// <summary>
		/// Exits a protected region of code, unconditionally transferring control to a specific
		/// target instruction.
		/// </summary>
		public static ILGenerator leave(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Leave, arg1);
			return il;
		}


		/// <summary>
		/// Exits a protected region of code, unconditionally transferring control to a target
		/// instruction (short form).
		/// </summary>
		public static ILGenerator leave_s(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Leave_S, arg1);
			return il;
		}


		/// <summary>
		/// Stores a value of type native int at a supplied address.
		/// </summary>
		public static ILGenerator stind_i(this ILGenerator il)
		{
			il.Emit(OpCodes.Stind_I);
			return il;
		}


		/// <summary>
		/// Converts the value on top of the evaluation stack to unsigned native int, and extends
		/// it to native int.
		/// </summary>
		public static ILGenerator conv_u(this ILGenerator il)
		{
			il.Emit(OpCodes.Conv_U);
			return il;
		}


		/// <summary>
		/// Returns an unmanaged pointer to the argument list of the current method.
		/// </summary>
		public static ILGenerator arglist(this ILGenerator il)
		{
			il.Emit(OpCodes.Arglist);
			return il;
		}


		/// <summary>
		/// Compares two values. If they are equal, the integer value 1 (int32) is pushed onto
		/// the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
		/// </summary>
		public static ILGenerator ceq(this ILGenerator il)
		{
			il.Emit(OpCodes.Ceq);
			return il;
		}


		/// <summary>
		/// Compares two values. If the first value is greater than the second, the integer value
		/// 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto
		/// the evaluation stack.
		/// </summary>
		public static ILGenerator cgt(this ILGenerator il)
		{
			il.Emit(OpCodes.Cgt);
			return il;
		}


		/// <summary>
		/// Compares two unsigned or unordered values. If the first value is greater than the
		/// second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise
		/// 0 (int32) is pushed onto the evaluation stack.
		/// </summary>
		public static ILGenerator cgt_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Cgt_Un);
			return il;
		}


		/// <summary>
		/// Compares two values. If the first value is less than the second, the integer value
		/// 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto
		/// the evaluation stack.
		/// </summary>
		public static ILGenerator clt(this ILGenerator il)
		{
			il.Emit(OpCodes.Clt);
			return il;
		}


		/// <summary>
		/// Compares the unsigned or unordered values value1 and value2. If value1 is less than
		/// value2, then the integer value 1 (int32) is pushed onto the evaluation stack; otherwise
		/// 0 (int32) is pushed onto the evaluation stack.
		/// </summary>
		public static ILGenerator clt_un(this ILGenerator il)
		{
			il.Emit(OpCodes.Clt_Un);
			return il;
		}


		/// <summary>
		/// Pushes an unmanaged pointer (type native int) to the native code implementing a specific
		/// method onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldftn(this ILGenerator il, MethodInfo arg1)
		{
			il.Emit(OpCodes.Ldftn, arg1);
			return il;
		}


		/// <summary>
		/// Pushes an unmanaged pointer (type native int) to the native code implementing a particular
		/// virtual method associated with a specified object onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldvirtftn(this ILGenerator il, MethodInfo arg1)
		{
			il.Emit(OpCodes.Ldvirtftn, arg1);
			return il;
		}


		/// <summary>
		/// Loads an argument (referenced by a specified index value) onto the stack.
		/// </summary>
		public static ILGenerator ldarg(this ILGenerator il, short arg1)
		{
			il.Emit(OpCodes.Ldarg, arg1);
			return il;
		}


		/// <summary>
		/// Load an argument address onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldarga(this ILGenerator il, short arg1)
		{
			il.Emit(OpCodes.Ldarga, arg1);
			return il;
		}


		/// <summary>
		/// Stores the value on top of the evaluation stack in the argument slot at a specified
		/// index.
		/// </summary>
		public static ILGenerator starg(this ILGenerator il, short arg1)
		{
			il.Emit(OpCodes.Starg, arg1);
			return il;
		}


		/// <summary>
		/// Loads the local variable at a specific index onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldloc(this ILGenerator il, LocalBuilder arg1)
		{
			il.Emit(OpCodes.Ldloc, arg1);
			return il;
		}


		/// <summary>
		/// Loads the local variable at a specific index onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldloc(this ILGenerator il, short arg1)
		{
			il.Emit(OpCodes.Ldloc, arg1);
			return il;
		}


		/// <summary>
		/// Loads the address of the local variable at a specific index onto the evaluation stack.
		/// </summary>
		public static ILGenerator ldloca(this ILGenerator il, short arg1)
		{
			il.Emit(OpCodes.Ldloca, arg1);
			return il;
		}


		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in a the
		/// local variable list at a specified index.
		/// </summary>
		public static ILGenerator stloc(this ILGenerator il, LocalBuilder arg1)
		{
			il.Emit(OpCodes.Stloc, arg1);
			return il;
		}


		/// <summary>
		/// Pops the current value from the top of the evaluation stack and stores it in a the
		/// local variable list at a specified index.
		/// </summary>
		public static ILGenerator stloc(this ILGenerator il, short arg1)
		{
			il.Emit(OpCodes.Stloc, arg1);
			return il;
		}


		/// <summary>
		/// Allocates a certain number of bytes from the local dynamic memory pool and pushes
		/// the address (a transient pointer, type *) of the first allocated byte onto the evaluation
		/// stack.
		/// </summary>
		public static ILGenerator localloc(this ILGenerator il)
		{
			il.Emit(OpCodes.Localloc);
			return il;
		}


		/// <summary>
		/// Transfers control from the filter clause of an exception back to the Common Language
		/// Infrastructure (CLI) exception handler.
		/// </summary>
		public static ILGenerator endfilter(this ILGenerator il)
		{
			il.Emit(OpCodes.Endfilter);
			return il;
		}


		/// <summary>
		/// Indicates that an address currently atop the evaluation stack might not be aligned
		/// to the natural size of the immediately following ldind, stind, ldfld, stfld, ldobj,
		/// stobj, initblk, or cpblk instruction.
		/// </summary>
		public static ILGenerator unaligned(this ILGenerator il, Label arg1)
		{
			il.Emit(OpCodes.Unaligned, arg1);
			return il;
		}


		/// <summary>
		/// Indicates that an address currently atop the evaluation stack might not be aligned
		/// to the natural size of the immediately following ldind, stind, ldfld, stfld, ldobj,
		/// stobj, initblk, or cpblk instruction.
		/// </summary>
		public static ILGenerator unaligned(this ILGenerator il, long arg1)
		{
			il.Emit(OpCodes.Unaligned, arg1);
			return il;
		}


		/// <summary>
		/// Specifies that an address currently atop the evaluation stack might be volatile,
		/// and the results of reading that location cannot be cached or that multiple stores
		/// to that location cannot be suppressed.
		/// </summary>
		public static ILGenerator volatile_(this ILGenerator il)
		{
			il.Emit(OpCodes.Volatile);
			return il;
		}


		/// <summary>
		/// Performs a postfixed method call instruction such that the current method's stack
		/// frame is removed before the actual call instruction is executed.
		/// </summary>
		public static ILGenerator tailcall(this ILGenerator il)
		{
			il.Emit(OpCodes.Tailcall);
			return il;
		}


		/// <summary>
		/// Initializes each field of the value type at a specified address to a null reference
		/// or a 0 of the appropriate primitive type.
		/// </summary>
		public static ILGenerator initobj(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Initobj, arg1);
			return il;
		}


		/// <summary>
		/// Constrains the type on which a virtual method call is made.
		/// </summary>
		public static ILGenerator constrained(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Constrained, arg1);
			return il;
		}


		/// <summary>
		/// Copies a specified number bytes from a source address to a destination address.
		/// </summary>
		public static ILGenerator cpblk(this ILGenerator il)
		{
			il.Emit(OpCodes.Cpblk);
			return il;
		}


		/// <summary>
		/// Initializes a specified block of memory at a specific address to a given size and
		/// initial value.
		/// </summary>
		public static ILGenerator initblk(this ILGenerator il)
		{
			il.Emit(OpCodes.Initblk);
			return il;
		}


		/// <summary>
		/// Rethrows the current exception.
		/// </summary>
		public static ILGenerator rethrow(this ILGenerator il)
		{
			il.Emit(OpCodes.Rethrow);
			return il;
		}


		/// <summary>
		/// Pushes the size, in bytes, of a supplied value type onto the evaluation stack.
		/// </summary>
		public static ILGenerator sizeof_(this ILGenerator il, Type arg1)
		{
			il.Emit(OpCodes.Sizeof, arg1);
			return il;
		}


		/// <summary>
		/// Retrieves the type token embedded in a typed reference.
		/// </summary>
		public static ILGenerator refanytype(this ILGenerator il)
		{
			il.Emit(OpCodes.Refanytype);
			return il;
		}


		/// <summary>
		/// Specifies that the subsequent array address operation performs no type check at run
		/// time, and that it returns a managed pointer whose mutability is restricted.
		/// </summary>
		public static ILGenerator readonly_(this ILGenerator il)
		{
			il.Emit(OpCodes.Readonly);
			return il;
		}
	}
}
