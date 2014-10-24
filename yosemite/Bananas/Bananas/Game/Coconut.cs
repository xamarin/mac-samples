using System;
using CoreAnimation;
using Foundation;
using SceneKit;

namespace Bananas
{
	public class Coconut : SCNNode
	{
		static SCNNode coconutProtoObject;
		static Coconut coconutThrowProtoObject;
		static SCNPhysicsShape coconutPhysicShape;

		public static SCNNode CoconutProtoObject {
			get {
				if (coconutProtoObject == null) {
					string coconutDaeName = GameSimulation.PathForArtResource ("characters/monkey/coconut.dae");
					coconutProtoObject = GameSimulation.LoadNodeWithName ("Coconut", coconutDaeName);
				}

				// create and return a clone of our proto object.
				SCNNode coconut = coconutProtoObject.Clone ();
				coconut.Name = "coconut";

				return coconut;
			}
		}

		public static Coconut CoconutThrowProtoObject {
			get {
				if (coconutThrowProtoObject == null) {
					string coconutDaeName = GameSimulation.PathForArtResource ("characters/monkey/coconut_no_translation.dae");
					SCNNode node = GameSimulation.LoadNodeWithName ("coconut", coconutDaeName);
					coconutThrowProtoObject = new Coconut ();
					coconutThrowProtoObject.AddNodes (node.ChildNodes);

					foreach (var child in coconutThrowProtoObject.ChildNodes)
						foreach (SCNMaterial m in child.Geometry.Materials)
							m.LightingModelName = SCNLightingModel.Constant;
				}

				// create and return a clone of our proto object.
				Coconut coconut = (Coconut)coconutThrowProtoObject.Clone ();
				coconut.Name = "coconut_throw";
				return coconut;
			}
		}

		public static SCNPhysicsShape CoconutPhysicsShape {
			get {
				if (coconutPhysicShape == null) {
					SCNSphere sphere = SCNSphere.Create (25f);
					coconutPhysicShape = SCNPhysicsShape.Create (sphere, new NSDictionary ());
				}

				return coconutPhysicShape;
			}
		}
	}
}

