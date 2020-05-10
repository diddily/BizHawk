﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BizHawk.Client.Common;

namespace BizHawk.Client.EmuHawk
{
	public partial class LuaRegisteredFunctionsList : Form
	{
		private readonly LuaFunctionList _registeredFunctions;

		public LuaRegisteredFunctionsList(LuaFunctionList registeredFunctions)
		{
			_registeredFunctions = registeredFunctions;
			InitializeComponent();
		}

		public Point StartLocation { get; set; } = new Point(0, 0);

		public void UpdateValues()
		{
			PopulateListView();
		}

		private void LuaRegisteredFunctionsList_Load(object sender, EventArgs e)
		{
			if (StartLocation.X > 0 && StartLocation.Y > 0)
			{
				Location = StartLocation;
			}

			PopulateListView();
		}

		private void OK_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void PopulateListView()
		{
			FunctionView.Items.Clear();
			
			var functions = _registeredFunctions
				.OrderBy(f => f.Event)
				.ThenBy(f => f.Name);
			foreach (var nlf in functions)
			{
				var item = new ListViewItem { Text = nlf.Event };
				item.SubItems.Add(nlf.Name);
				item.SubItems.Add(nlf.Guid.ToString());
				FunctionView.Items.Add(item);
			}

			DoButtonsStatus();
		}

		private void CallButton_Click(object sender, EventArgs e)
		{
			CallFunction();
		}

		private void RemoveButton_Click(object sender, EventArgs e)
		{
			RemoveFunctionButton();
		}

		private void CallFunction()
		{
			var indices = FunctionView.SelectedIndices;
			if (indices.Count > 0)
			{
				foreach (int index in indices)
				{
					var guid = FunctionView.Items[index].SubItems[2].Text;
					_registeredFunctions[guid].Call();
				}
			}
		}

		private void RemoveFunctionButton()
		{
			var indices = FunctionView.SelectedIndices;
			if (indices.Count > 0)
			{
				foreach (int index in indices)
				{
					var guid = FunctionView.Items[index].SubItems[2].Text;
					var nlf = _registeredFunctions[guid];
					_registeredFunctions.Remove(nlf, Global.Emulator); // TODO: don't use Global
				}

				PopulateListView();
			}
		}

		private void FunctionView_SelectedIndexChanged(object sender, EventArgs e)
		{
			DoButtonsStatus();
		}

		private void FunctionView_DoubleClick(object sender, EventArgs e)
		{
			CallFunction();
		}

		private void RemoveAllBtn_Click(object sender, EventArgs e)
		{
			_registeredFunctions.Clear(Global.Emulator); // TODO: don't use Global
			PopulateListView();
		}

		private void DoButtonsStatus()
		{
			var indexes = FunctionView.SelectedIndices;
			CallButton.Enabled = indexes.Count > 0;
			RemoveButton.Enabled = indexes.Count > 0;
			RemoveAllBtn.Enabled = _registeredFunctions.Any();
		}

		private void FunctionView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && !e.Control && !e.Alt && !e.Shift) // Delete
			{
				RemoveFunctionButton();
			}
			else if (e.KeyCode == Keys.Space && !e.Control && !e.Alt && !e.Shift) // Space
			{
				CallFunction();
			}
			else if (e.KeyCode == Keys.Enter && !e.Control && !e.Alt && !e.Shift) // Enter
			{
				CallFunction();
			}
		}
	}
}
