using Godot;
using System;

public partial class UpdateBuffer_InputNode : Node
{
	public bool first = true; 
	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.W))
		{
			if (first)
			{
				GD.Print("W is being held");
				first = false;
				DoSome();
			}
		}
	}
	public void DoSome()
	{
		foreach(var test in UpdateBuffer.list)
		{
			//RenderingServer.CallOnRenderThread(new Callable(test, TestCompositorEffect.MethodName.Reverse));
			test.Reverse();
		}
		GD.Print(UpdateBuffer.list.Count);
	}
}
