using Sandbox;
using System;

public class SpectateCamera : Sandbox.Camera
{
	Angles LookAngles;

	Vector3 MoveInput;
	Vector3 TargetPos;

	Rotation TargetRot;

	bool PivotEnabled;

	float MoveSpeed;
	float FovOverride = 0;
	float LerpMode = 0;

	public override void Activated()
	{
		base.Activated();

		TargetPos = CurrentView.Position;
		TargetRot = CurrentView.Rotation;

		Pos = TargetPos;
		Rot = TargetRot;
		LookAngles = Rot.Angles();
		FovOverride = 80;

		DoFPoint = 0.0f;
		DoFBlurSize = 0.0f;
	}

	public override void Deactivated()
	{
		base.Deactivated();
	}

	public override void Update()
	{
		var player = Local.Client;
		if (player == null) 
			return;

		var tr = Trace.Ray(Pos, Pos + Rot.Forward * 4096).UseHitboxes().Run();

		FieldOfView = FovOverride;

		Viewer = null;
		{
			var lerpTarget = tr.EndPos.Distance(Pos);
			DoFPoint = lerpTarget;
		}

		FreeMove();
	}

	public override void BuildInput(InputBuilder input)
	{
		MoveInput = input.AnalogMove;

		MoveSpeed = 1;
		if (input.Down(InputButton.Run)) 
			MoveSpeed = 5;

		LookAngles += input.AnalogLook * (FovOverride / 80.0f);
		LookAngles.roll = 0;

		PivotEnabled = input.Down(InputButton.Walk);

		input.Clear();
		input.StopProcessing = true;
	}

	void FreeMove()
	{
		var mv = MoveInput.Normal * 300 * RealTime.Delta * Rot * MoveSpeed;

		TargetRot = Rotation.From(LookAngles);
		TargetPos += mv;

		Pos = Vector3.Lerp(Pos, TargetPos, 10 * RealTime.Delta * (1 - LerpMode));
		Rot = Rotation.Slerp(Rot, TargetRot, 10 * RealTime.Delta * (1 - LerpMode));
	}
}