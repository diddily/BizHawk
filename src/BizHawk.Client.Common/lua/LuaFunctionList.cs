﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BizHawk.Emulation.Common;

namespace BizHawk.Client.Common
{
	public class LuaFunctionList : IEnumerable<NamedLuaFunction>
	{
		private readonly List<NamedLuaFunction> _functions = new List<NamedLuaFunction>();

		public NamedLuaFunction this[string guid] => 
			_functions.FirstOrDefault(nlf => nlf.Guid.ToString() == guid);

		public void Add(NamedLuaFunction nlf) => _functions.Add(nlf);

		public bool Remove(NamedLuaFunction function, IEmulator emulator)
		{
			if (emulator.InputCallbacksAvailable())
			{
				emulator.AsInputPollable().InputCallbacks.Remove(function.Callback);
			}

			if (emulator.MemoryCallbacksAvailable())
			{
				emulator.AsDebuggable().MemoryCallbacks.Remove(function.MemCallback);
			}

			return _functions.Remove(function);
		}

		public void RemoveForFile(LuaFile file, IEmulator emulator)
		{
			var functionsToRemove = _functions
				.ForFile(file)
				.ToList();

			foreach (var function in functionsToRemove)
			{
				Remove(function, emulator);
			}
		}

		public void Clear(IEmulator emulator)
		{
			if (emulator.InputCallbacksAvailable())
			{
				emulator.AsInputPollable().InputCallbacks.RemoveAll(_functions.Select(w => w.Callback));
			}

			if (emulator.MemoryCallbacksAvailable())
			{
				var memoryCallbacks = emulator.AsDebuggable().MemoryCallbacks;
				memoryCallbacks.RemoveAll(_functions.Select(w => w.MemCallback));
			}

			_functions.Clear();
		}

		public IEnumerator<NamedLuaFunction> GetEnumerator() => _functions.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _functions.GetEnumerator();
	}

	public static class LuaFunctionListExtensions
	{
		public static IEnumerable<NamedLuaFunction> ForFile(this IEnumerable<NamedLuaFunction> list, LuaFile luaFile)
		{
			return list
				.Where(l => l.LuaFile.Path == luaFile.Path
					|| l.LuaFile.Thread == luaFile.Thread);
		}

		public static IEnumerable<NamedLuaFunction> ForEvent(this IEnumerable<NamedLuaFunction> list, string eventName)
		{
			return list.Where(l => l.Event == eventName);
		}
	}
}
