using System;

using AppKit;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace SceneKitSessionWWDC2014
{
	public class SlideParticles : Slide
	{
		private enum ParticleSteps
		{
			First,
			Fire,
			FireScreen,
			Local,
			Gravity,
			Collider,
			Fields,
			FieldsVortex,
			SubSystems,
			Confetti,
			EmitterCube,
			EmitterSphere,
			EmitterTorus,
			Count
		}

		private SCNNode Hole { get; set; }

		private SCNNode Hole2 { get; set; }

		private SCNNode FloorNode { get; set; }

		private SCNNode BoxNode { get; set; }

		private SCNNode ParticleHolder { get; set; }

		private SCNNode FieldOwner { get; set; }

		private SCNNode VortexFieldOwner { get; set; }

		private SCNParticleSystem Snow { get; set; }

		private SCNParticleSystem Bokeh { get; set; }

		private const int HOLE_Z = 10;

		private const double VS = 20.0;

		private const double VW = 10.0;

		public override int NumberOfSteps ()
		{
			return (int)ParticleSteps.Count;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Particles");
			TextManager.SetSubtitle ("SCNParticleSystem");
			TextManager.AddBulletAtLevel ("Achieve a large number of effects", 0);
			TextManager.AddBulletAtLevel ("3D particle editor built into Xcode", 0);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case (int)ParticleSteps.Fire:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

				TextManager.AddEmptyLine ();
				TextManager.AddBulletAtLevel ("Particle Image", 0);
				TextManager.AddBulletAtLevel ("Color over life duration", 0);
				TextManager.AddBulletAtLevel ("Size over life duration", 0);
				TextManager.AddBulletAtLevel ("Several blend modes", 0);

				TextManager.FlipInText (SlideTextManager.TextType.Bullet);

				var hole = SCNNode.Create ();
				hole.Geometry = SCNTube.Create (1.7f, 1.9f, 1.5f);
				hole.Position = new SCNVector3 (0, 0, HOLE_Z);
				hole.Scale = new SCNVector3 (1, 0, 1);

				GroundNode.AddChildNode (hole);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.5f;

				hole.Scale = new SCNVector3 (1, 1, 1);

				SCNTransaction.Commit ();

				var ps = SCNParticleSystem.Create ("fire", "Particles");
				hole.AddParticleSystem (ps);

				Hole = hole;
				break;
			case (int)ParticleSteps.FireScreen:
				ps = Hole.ParticleSystems [0];
				ps.BlendMode = SCNParticleBlendMode.Screen;
				break;
			case (int)ParticleSteps.Local:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

				TextManager.AddBulletAtLevel ("Local vs Global", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);

				Hole.RemoveAllParticleSystems ();
				Hole2 = Hole.Clone ();
				Hole2.Geometry = (SCNGeometry)Hole.Geometry.Copy ();
				Hole2.Position = new SCNVector3 (0, -2, HOLE_Z - 4);
				GroundNode.AddChildNode (Hole2);
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.5;
				Hole2.Position = new SCNVector3 (0, 0, HOLE_Z - 4);
				SCNTransaction.Commit ();

				ps = SCNParticleSystem.Create ("reactor", "Particles");
				ps.ParticleColorVariation = new SCNVector4 (0, 0, 0.5f, 0);
				Hole.AddParticleSystem (ps);

				var localPs = (SCNParticleSystem)ps.Copy ();
				localPs.ParticleImage = ps.ParticleImage;
				localPs.Local = true;
				Hole2.AddParticleSystem (localPs);

				var animation = CABasicAnimation.FromKeyPath ("position");
				animation.From = NSValue.FromVector (new SCNVector3 (7, 0, HOLE_Z));
				animation.To = NSValue.FromVector (new SCNVector3 (-7, 0, HOLE_Z));
				animation.BeginTime = CAAnimation.CurrentMediaTime () + 0.75;
				animation.Duration = 8;
				animation.AutoReverses = true;
				animation.RepeatCount = float.MaxValue;
				animation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				animation.TimeOffset = animation.Duration / 2;
				Hole.AddAnimation (animation, new NSString ("animateHole"));

				animation = CABasicAnimation.FromKeyPath ("position");
				animation.From = NSValue.FromVector (new SCNVector3 (-7, 0, HOLE_Z - 4));
				animation.To = NSValue.FromVector (new SCNVector3 (7, 0, HOLE_Z - 4));
				animation.BeginTime = CAAnimation.CurrentMediaTime () + 0.75;
				animation.Duration = 8;
				animation.AutoReverses = true;
				animation.RepeatCount = float.MaxValue;
				animation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				animation.TimeOffset = animation.Duration / 2;
				Hole2.AddAnimation (animation, new NSString ("animateHole"));
				break;
			case (int)ParticleSteps.Gravity:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

				TextManager.AddBulletAtLevel ("Affected by gravity", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);

				Hole2.RemoveAllParticleSystems ();
				Hole2.RunAction (SCNAction.Sequence (new SCNAction[] {
					SCNAction.ScaleTo (0, 0.5),
					SCNAction.RemoveFromParentNode ()
				}));
				Hole.RemoveAllParticleSystems ();
				Hole.RemoveAnimation (new NSString ("animateHole"), 0.5f);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.5;

				var tube = (SCNTube)Hole.Geometry;
				tube.InnerRadius = 0.3f;
				tube.OuterRadius = 0.4f;
				tube.Height = 1.0f;

				SCNTransaction.Commit ();

				ps = SCNParticleSystem.Create ("sparks", "Particles");
				Hole.RemoveAllParticleSystems ();
				Hole.AddParticleSystem (ps);

				foreach (var child in ((SCNView)presentationViewController.View).Scene.RootNode.ChildNodes) {
					if (child.Geometry != null) {
						if (child.Geometry.GetType () == typeof(SCNFloor))
							FloorNode = child;
					}
				}

				/*FloorNode = ((SCNView)presentationViewController.View).Scene.RootNode.FindNodes ((SCNNode child, out bool stop) => {
					stop = false;
					if (child.Geometry != null)
						stop = (child.Geometry.GetType () == typeof(SCNFloor));
					return stop;
				});*/

				/*FloorNode = [presentationViewController.view.scene.rootNode childNodesPassingTest:^BOOL(SCNNode *child, BOOL *stop) {
					return [child.geometry isKindOfClass:[SCNFloor class]];
				}][0];*/

				ps.ColliderNodes = new SCNNode[] { FloorNode };

				break;
			case (int)ParticleSteps.Collider:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

				TextManager.AddBulletAtLevel ("Affected by colliders", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);

				var boxNode = SCNNode.Create ();
				boxNode.Geometry = SCNBox.Create (5, 0.2f, 5, 0);
				boxNode.Position = new SCNVector3 (0, 7, HOLE_Z);
				boxNode.Geometry.FirstMaterial.Emission.Contents = NSColor.DarkGray;

				GroundNode.AddChildNode (boxNode);

				ps = Hole.ParticleSystems [0];
				ps.ColliderNodes = new SCNNode[] { FloorNode, boxNode };

				animation = CABasicAnimation.FromKeyPath ("eulerAngles");
				animation.From = NSValue.FromVector (new SCNVector3 (0, 0, (nfloat)Math.PI / 4 * 1.7f));
				animation.To = NSValue.FromVector (new SCNVector3 (0, 0, -(nfloat)Math.PI / 4 * 1.7f));
				animation.BeginTime = CAAnimation.CurrentMediaTime () + 0.5;
				animation.Duration = 2;
				animation.AutoReverses = true;
				animation.RepeatCount = float.MaxValue;
				animation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				animation.TimeOffset = animation.Duration / 2;
				boxNode.AddAnimation (animation, new NSString ("animateHole"));

				BoxNode = boxNode;
				break;
			case (int)ParticleSteps.Fields:
				Hole.RemoveAllParticleSystems ();

				Hole.RunAction (SCNAction.Sequence (new SCNAction[] {
					SCNAction.ScaleTo (0, 0.75),
					SCNAction.RemoveFromParentNode ()
				}));

				BoxNode.RunAction (SCNAction.Sequence (new SCNAction[] {
					SCNAction.MoveBy (0, 15, 0, 1.0),
					SCNAction.RemoveFromParentNode ()
				}));

				var particleHolder = SCNNode.Create ();
				particleHolder.Position = new SCNVector3 (0, 20, HOLE_Z);
				GroundNode.AddChildNode (particleHolder);

				ParticleHolder = particleHolder;

				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.AddBulletAtLevel ("Affected by physics fields", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);

				ps = SCNParticleSystem.Create ("snow", "Particles");
				ps.AffectedByPhysicsFields = true;
				ParticleHolder.AddParticleSystem (ps);
				Snow = ps;

				//physics field
				var field = SCNPhysicsField.CreateTurbulenceField (50, 1);
				field.HalfExtent = new SCNVector3 (20, 20, 20);
				field.Strength = 4.0f;

				var fieldOwner = SCNNode.Create ();
				fieldOwner.Position = new SCNVector3 (0, 5, HOLE_Z);

				GroundNode.AddChildNode (fieldOwner);
				fieldOwner.PhysicsField = field;
				FieldOwner = fieldOwner;

				ps.ColliderNodes = new SCNNode[] { FloorNode };
				break;
			case (int)ParticleSteps.FieldsVortex:
				VortexFieldOwner = SCNNode.Create ();
				VortexFieldOwner.Position = new SCNVector3 (0, 5, HOLE_Z);

				GroundNode.AddChildNode (VortexFieldOwner);

				//tornado
				var worldOrigin = new SCNVector3 (FieldOwner.WorldTransform.M41, FieldOwner.WorldTransform.M42, FieldOwner.WorldTransform.M43);
				var worldAxis = new SCNVector3 (0, 1, 0);

				var vortex = SCNPhysicsField.CustomField ((SCNVector3 position, SCNVector3 velocity, float mass, float charge, double timeInSeconds) => {
					var l = new SCNVector3 ();
					l.X = worldOrigin.X - position.X;
					l.Z = worldOrigin.Z - position.Z;
					SCNVector3 t = Cross (worldAxis, l);
					var d2 = (l.X * l.X + l.Z * l.Z);
					var vs = (nfloat)(VS / Math.Sqrt (d2));
					var fy = (nfloat)(1.0 - (Math.Min (1.0, (position.Y / 15.0))));
					return new SCNVector3 (t.X * vs + l.X * (nfloat)VW * fy, 0, t.Z * vs + l.Z * (nfloat)VW * fy);
				});
				vortex.HalfExtent = new SCNVector3 (100, 100, 100);
				VortexFieldOwner.PhysicsField = vortex;
				break;
			case (int)ParticleSteps.SubSystems:
				FieldOwner.RemoveFromParentNode ();
				ParticleHolder.RemoveAllParticleSystems ();
				Snow.DampingFactor = -1;

				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.AddBulletAtLevel ("Sub-particle system on collision", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);

				ps = SCNParticleSystem.Create ("rain", "Particles");
				var pss = SCNParticleSystem.Create ("plok", "Particles");
				pss.IdleDuration = 0;
				pss.Loops = false;

				ps.SystemSpawnedOnCollision = pss;

				ParticleHolder.AddParticleSystem (ps);
				ps.ColliderNodes = new SCNNode[] { FloorNode };
				break;
			case (int)ParticleSteps.Confetti:
				ParticleHolder.RemoveAllParticleSystems ();

				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.AddBulletAtLevel ("Custom blocks", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);

				ps = SCNParticleSystem.Create ();
				ps.EmitterShape = SCNBox.Create (20, 9, 5, 0);
				ps.BirthRate = 100;
				ps.ParticleLifeSpan = 10;
				ps.ParticleLifeSpanVariation = 0;
				ps.SpreadingAngle = 20;
				ps.ParticleSize = 0.25f;
				ps.ParticleVelocity = 10;
				ps.ParticleVelocityVariation = 19;
				ps.BirthDirection = SCNParticleBirthDirection.Constant;
				ps.EmittingDirection = new SCNVector3 (0, -1, 0);
				ps.BirthLocation = SCNParticleBirthLocation.Volume;
				ps.ParticleImage = new NSImage (NSBundle.MainBundle.PathForResource ("Particles/confetti", "png"));
				ps.LightingEnabled = true;
				ps.OrientationMode = SCNParticleOrientationMode.Free;
				ps.SortingMode = SCNParticleSortingMode.Distance;
				ps.ParticleAngleVariation = 180;
				ps.ParticleAngularVelocity = 200;
				ps.ParticleAngularVelocityVariation = 400;
				ps.ParticleColor = NSColor.Green;
				ps.ParticleColorVariation = new SCNVector4 (0.2f, 0.1f, 0.1f, 0);
				ps.ParticleBounce = 0;
				ps.ParticleFriction = 0.6f;
				ps.ColliderNodes = new SCNNode[] { FloorNode };
				ps.BlendMode = SCNParticleBlendMode.Alpha;

				var floatAnimation = CAKeyFrameAnimation.FromKeyPath ("");
				floatAnimation.Values = new NSNumber[] { 1, 1, 0 };
				floatAnimation.KeyTimes = new NSNumber[] { 0, 0.9f, 1 };
				floatAnimation.Duration = 1.0f;
				floatAnimation.Additive = false;

				//ps.PropertyControllers = @{ SCNParticlePropertyOpacity: [SCNParticlePropertyController controllerWithAnimation:floatAnimation] };
				//ps.HandleEvent (SCNParticleEvent.Birth, 
				/*[ps handleEvent:SCNParticleEventBirth forProperties:@[SCNParticlePropertyColor] withBlock:^(void **data, size_t *dataStride, uint32_t *indices , NSInteger count) {

					for (int i = 0; i < count; ++i) {
						var col = (float *)((char *)data[0] + dataStride[0] * i);
						if (rand() & 0x1) { // swith green for red
							col[0] = col[1];
							col[1] = 0;
						}

					}
				}];*/

				/*[ps handleEvent:SCNParticleEventCollision forProperties:@[SCNParticlePropertyAngle, SCNParticlePropertyRotationAxis, SCNParticlePropertyAngularVelocity, SCNParticlePropertyVelocity, SCNParticlePropertyContactNormal] withBlock:^(void **data, size_t *dataStride, uint32_t *indices , NSInteger count) {

					for (NSInteger i = 0; i < count; ++i) {
						// fix orientation
						float *angle = (float *)((char *)data[0] + dataStride[0] * indices[i]);
						float *axis = (float *)((char *)data[1] + dataStride[1] * indices[i]);

						float *colNrm = (float *)((char *)data[4] + dataStride[4] * indices[i]);
						SCNVector3 collisionNormal = {colNrm[0], colNrm[1], colNrm[2]};
						SCNVector3 cp = SCNVector3CrossProduct(collisionNormal, SCNVector3Make(0, 0, 1));
						CGFloat cpLen = SCNVector3Length(cp);
						angle[0] = asin(cpLen);

						axis[0] = cp.x / cpLen;
						axis[1] = cp.y / cpLen;
						axis[2] = cp.z / cpLen;

						// kill angular rotation
						float *angVel = (float *)((char *)data[2] + dataStride[2] * indices[i]);
						angVel[0] = 0;

						if (colNrm[1] > 0.4) {
							float *vel = (float *)((char *)data[3] + dataStride[3] * indices[i]);
							vel[0] = 0;
							vel[1] = 0;
							vel[2] = 0;
						}
					}
				}];*/

				ParticleHolder.AddParticleSystem (ps);
				break;
			case (int)ParticleSteps.EmitterCube:
				ParticleHolder.RemoveAllParticleSystems ();

				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.AddBulletAtLevel ("Emitter shape", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);

				ParticleHolder.RemoveFromParentNode ();

				ps = SCNParticleSystem.Create ("emitters", "Particles");
				ps.Local = true;
				ParticleHolder.AddParticleSystem (ps);

				var node = SCNNode.Create ();
				node.Position = new SCNVector3 (3, 6, HOLE_Z);
				node.RunAction (SCNAction.RepeatActionForever (SCNAction.RotateBy ((float)Math.PI * 2, new SCNVector3 (0.3f, 1, 0), 8)));
				GroundNode.AddChildNode (node);
				Bokeh = ps;

				node.AddParticleSystem (ps);
				break;
			case (int)ParticleSteps.EmitterSphere:
				Bokeh.EmitterShape = SCNSphere.Create (5);
				break;
			case (int)ParticleSteps.EmitterTorus:
				Bokeh.EmitterShape = SCNTorus.Create (5, 1);
				break;
			}
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			((SCNView)presentationViewController.View).Scene.RemoveAllParticleSystems ();
		}

		private static SCNVector3 Cross (SCNVector3 a, SCNVector3 b)
		{
			SCNVector3 c;

			c.X = a.Y * b.Z - a.Z * b.Y;
			c.Y = a.Z * b.X - a.X * b.Z;
			c.Z = a.X * b.Y - a.Y * b.X;

			return c;
		}
	}
}

