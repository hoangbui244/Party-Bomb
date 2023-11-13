using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Facepunch
{
	[ExecuteInEditMode]
	[AddComponentMenu("Rendering/Highlight")]
	public class Highlight : MonoBehaviour
	{
		private const CameraEvent CameraEventTarget = CameraEvent.AfterForwardAlpha;

		public bool VisualizeRenderTarget;

		public Camera TargetCamera;

		public Material ImageEffectMaterial;

		public Material MeshRenderMaterial;

		private CommandBuffer commandBuffer;

		private bool isDirty;

		private int _HighlightTextureId;

		public List<Renderer> Highlighted = new List<Renderer>();

		public bool ClearAll()
		{
			if (Highlighted.Count == 0)
			{
				return false;
			}
			Highlighted.Clear();
			isDirty = true;
			return true;
		}

		public bool AddRenderer(Renderer r)
		{
			if (Highlighted.Contains(r))
			{
				return false;
			}
			Highlighted.Add(r);
			isDirty = true;
			return true;
		}

		public bool RemoveRenderer(Renderer r)
		{
			if (!Highlighted.Contains(r))
			{
				return false;
			}
			Highlighted.Remove(r);
			isDirty = true;
			return true;
		}

		public bool Rebuild(bool force = false)
		{
			if (isDirty | force)
			{
				isDirty = false;
				RebuildCommandBuffer();
				return true;
			}
			return false;
		}

		private void Start()
		{
			_HighlightTextureId = Shader.PropertyToID("_HighlightTexture");
		}

		private void OnEnable()
		{
			RebuildCommandBuffer();
		}

		private void OnDisable()
		{
			ReleaseAll();
		}

		private void RebuildCommandBuffer()
		{
			if (!(TargetCamera == null))
			{
				if (commandBuffer == null)
				{
					commandBuffer = new CommandBuffer();
					commandBuffer.name = "Highlight Render";
					TargetCamera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, commandBuffer);
				}
				commandBuffer.Clear();
				if (!(MeshRenderMaterial == null) && !(ImageEffectMaterial == null))
				{
					commandBuffer.GetTemporaryRT(_HighlightTextureId, TargetCamera.pixelWidth, TargetCamera.pixelHeight, 24, FilterMode.Point, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
					commandBuffer.SetRenderTarget(_HighlightTextureId);
					commandBuffer.ClearRenderTarget(clearDepth: true, clearColor: true, Color.black);
					foreach (Renderer item in Highlighted)
					{
						if ((bool)item)
						{
							commandBuffer.DrawRenderer(item, MeshRenderMaterial);
						}
					}
					commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
					commandBuffer.Blit(_HighlightTextureId, BuiltinRenderTextureType.CameraTarget, VisualizeRenderTarget ? null : ImageEffectMaterial);
					commandBuffer.ReleaseTemporaryRT(_HighlightTextureId);
				}
			}
		}

		public void ReleaseAll()
		{
			if (TargetCamera != null && commandBuffer != null)
			{
				TargetCamera.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, commandBuffer);
				commandBuffer = null;
			}
		}

		private void OnValidate()
		{
			if (TargetCamera == null)
			{
				TargetCamera = GetComponent<Camera>();
			}
			if (TargetCamera != null && TargetCamera.depthTextureMode == DepthTextureMode.None)
			{
				TargetCamera.depthTextureMode = DepthTextureMode.Depth;
			}
			ReleaseAll();
			RebuildCommandBuffer();
		}
	}
}
