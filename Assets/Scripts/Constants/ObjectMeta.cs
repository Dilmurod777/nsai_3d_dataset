using UnityEngine;

namespace Constants
{
	public enum RotationAxisEnum
	{
		X,
		Y,
		Z,
		negX, 
		negY,
		negZ
	}
	public class ObjectMeta : MonoBehaviour
	{
		public RotationAxisEnum AttachRotationAxis;
		public RotationAxisEnum DettachRotationAxis;
		public GeneralConstants.AttachTypes AttachType;
		public GeneralConstants.AttachTypes DetachType;
	}
}