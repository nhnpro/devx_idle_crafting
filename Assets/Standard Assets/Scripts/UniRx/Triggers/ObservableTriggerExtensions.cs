using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Triggers
{
	public static class ObservableTriggerExtensions
	{
		public static UniRx.IObservable<int> OnAnimatorIKAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<int>();
			}
			return GetOrAddComponent<ObservableAnimatorTrigger>(component.gameObject).OnAnimatorIKAsObservable();
		}

		public static UniRx.IObservable<Unit> OnAnimatorMoveAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableAnimatorTrigger>(component.gameObject).OnAnimatorMoveAsObservable();
		}

		public static UniRx.IObservable<Collision2D> OnCollisionEnter2DAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Collision2D>();
			}
			return GetOrAddComponent<ObservableCollision2DTrigger>(component.gameObject).OnCollisionEnter2DAsObservable();
		}

		public static UniRx.IObservable<Collision2D> OnCollisionExit2DAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Collision2D>();
			}
			return GetOrAddComponent<ObservableCollision2DTrigger>(component.gameObject).OnCollisionExit2DAsObservable();
		}

		public static UniRx.IObservable<Collision2D> OnCollisionStay2DAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Collision2D>();
			}
			return GetOrAddComponent<ObservableCollision2DTrigger>(component.gameObject).OnCollisionStay2DAsObservable();
		}

		public static UniRx.IObservable<Collision> OnCollisionEnterAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Collision>();
			}
			return GetOrAddComponent<ObservableCollisionTrigger>(component.gameObject).OnCollisionEnterAsObservable();
		}

		public static UniRx.IObservable<Collision> OnCollisionExitAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Collision>();
			}
			return GetOrAddComponent<ObservableCollisionTrigger>(component.gameObject).OnCollisionExitAsObservable();
		}

		public static UniRx.IObservable<Collision> OnCollisionStayAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Collision>();
			}
			return GetOrAddComponent<ObservableCollisionTrigger>(component.gameObject).OnCollisionStayAsObservable();
		}

		public static UniRx.IObservable<Unit> OnDestroyAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Return(Unit.Default);
			}
			return GetOrAddComponent<ObservableDestroyTrigger>(component.gameObject).OnDestroyAsObservable();
		}

		public static UniRx.IObservable<Unit> OnEnableAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableEnableTrigger>(component.gameObject).OnEnableAsObservable();
		}

		public static UniRx.IObservable<Unit> OnDisableAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableEnableTrigger>(component.gameObject).OnDisableAsObservable();
		}

		public static UniRx.IObservable<Unit> FixedUpdateAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableFixedUpdateTrigger>(component.gameObject).FixedUpdateAsObservable();
		}

		public static UniRx.IObservable<Unit> LateUpdateAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableLateUpdateTrigger>(component.gameObject).LateUpdateAsObservable();
		}

		public static UniRx.IObservable<Collider2D> OnTriggerEnter2DAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Collider2D>();
			}
			return GetOrAddComponent<ObservableTrigger2DTrigger>(component.gameObject).OnTriggerEnter2DAsObservable();
		}

		public static UniRx.IObservable<Collider2D> OnTriggerExit2DAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Collider2D>();
			}
			return GetOrAddComponent<ObservableTrigger2DTrigger>(component.gameObject).OnTriggerExit2DAsObservable();
		}

		public static UniRx.IObservable<Collider2D> OnTriggerStay2DAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Collider2D>();
			}
			return GetOrAddComponent<ObservableTrigger2DTrigger>(component.gameObject).OnTriggerStay2DAsObservable();
		}

		public static UniRx.IObservable<Collider> OnTriggerEnterAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Collider>();
			}
			return GetOrAddComponent<ObservableTriggerTrigger>(component.gameObject).OnTriggerEnterAsObservable();
		}

		public static UniRx.IObservable<Collider> OnTriggerExitAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Collider>();
			}
			return GetOrAddComponent<ObservableTriggerTrigger>(component.gameObject).OnTriggerExitAsObservable();
		}

		public static UniRx.IObservable<Collider> OnTriggerStayAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Collider>();
			}
			return GetOrAddComponent<ObservableTriggerTrigger>(component.gameObject).OnTriggerStayAsObservable();
		}

		public static UniRx.IObservable<Unit> UpdateAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableUpdateTrigger>(component.gameObject).UpdateAsObservable();
		}

		public static UniRx.IObservable<Unit> OnBecameInvisibleAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableVisibleTrigger>(component.gameObject).OnBecameInvisibleAsObservable();
		}

		public static UniRx.IObservable<Unit> OnBecameVisibleAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableVisibleTrigger>(component.gameObject).OnBecameVisibleAsObservable();
		}

		public static UniRx.IObservable<Unit> OnBeforeTransformParentChangedAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableTransformChangedTrigger>(component.gameObject).OnBeforeTransformParentChangedAsObservable();
		}

		public static UniRx.IObservable<Unit> OnTransformParentChangedAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableTransformChangedTrigger>(component.gameObject).OnTransformParentChangedAsObservable();
		}

		public static UniRx.IObservable<Unit> OnTransformChildrenChangedAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableTransformChangedTrigger>(component.gameObject).OnTransformChildrenChangedAsObservable();
		}

		public static UniRx.IObservable<Unit> OnCanvasGroupChangedAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableCanvasGroupChangedTrigger>(component.gameObject).OnCanvasGroupChangedAsObservable();
		}

		public static UniRx.IObservable<Unit> OnRectTransformDimensionsChangeAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableRectTransformTrigger>(component.gameObject).OnRectTransformDimensionsChangeAsObservable();
		}

		public static UniRx.IObservable<Unit> OnRectTransformRemovedAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableRectTransformTrigger>(component.gameObject).OnRectTransformRemovedAsObservable();
		}

		public static UniRx.IObservable<BaseEventData> OnDeselectAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<BaseEventData>();
			}
			return GetOrAddComponent<ObservableDeselectTrigger>(component.gameObject).OnDeselectAsObservable();
		}

		public static UniRx.IObservable<AxisEventData> OnMoveAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<AxisEventData>();
			}
			return GetOrAddComponent<ObservableMoveTrigger>(component.gameObject).OnMoveAsObservable();
		}

		public static UniRx.IObservable<PointerEventData> OnPointerDownAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<PointerEventData>();
			}
			return GetOrAddComponent<ObservablePointerDownTrigger>(component.gameObject).OnPointerDownAsObservable();
		}

		public static UniRx.IObservable<PointerEventData> OnPointerEnterAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<PointerEventData>();
			}
			return GetOrAddComponent<ObservablePointerEnterTrigger>(component.gameObject).OnPointerEnterAsObservable();
		}

		public static UniRx.IObservable<PointerEventData> OnPointerExitAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<PointerEventData>();
			}
			return GetOrAddComponent<ObservablePointerExitTrigger>(component.gameObject).OnPointerExitAsObservable();
		}

		public static UniRx.IObservable<PointerEventData> OnPointerUpAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<PointerEventData>();
			}
			return GetOrAddComponent<ObservablePointerUpTrigger>(component.gameObject).OnPointerUpAsObservable();
		}

		public static UniRx.IObservable<BaseEventData> OnSelectAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<BaseEventData>();
			}
			return GetOrAddComponent<ObservableSelectTrigger>(component.gameObject).OnSelectAsObservable();
		}

		public static UniRx.IObservable<PointerEventData> OnPointerClickAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<PointerEventData>();
			}
			return GetOrAddComponent<ObservablePointerClickTrigger>(component.gameObject).OnPointerClickAsObservable();
		}

		public static UniRx.IObservable<BaseEventData> OnSubmitAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<BaseEventData>();
			}
			return GetOrAddComponent<ObservableSubmitTrigger>(component.gameObject).OnSubmitAsObservable();
		}

		public static UniRx.IObservable<PointerEventData> OnDragAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<PointerEventData>();
			}
			return GetOrAddComponent<ObservableDragTrigger>(component.gameObject).OnDragAsObservable();
		}

		public static UniRx.IObservable<PointerEventData> OnBeginDragAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<PointerEventData>();
			}
			return GetOrAddComponent<ObservableBeginDragTrigger>(component.gameObject).OnBeginDragAsObservable();
		}

		public static UniRx.IObservable<PointerEventData> OnEndDragAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<PointerEventData>();
			}
			return GetOrAddComponent<ObservableEndDragTrigger>(component.gameObject).OnEndDragAsObservable();
		}

		public static UniRx.IObservable<PointerEventData> OnDropAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<PointerEventData>();
			}
			return GetOrAddComponent<ObservableDropTrigger>(component.gameObject).OnDropAsObservable();
		}

		public static UniRx.IObservable<BaseEventData> OnUpdateSelectedAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<BaseEventData>();
			}
			return GetOrAddComponent<ObservableUpdateSelectedTrigger>(component.gameObject).OnUpdateSelectedAsObservable();
		}

		public static UniRx.IObservable<PointerEventData> OnInitializePotentialDragAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<PointerEventData>();
			}
			return GetOrAddComponent<ObservableInitializePotentialDragTrigger>(component.gameObject).OnInitializePotentialDragAsObservable();
		}

		public static UniRx.IObservable<BaseEventData> OnCancelAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<BaseEventData>();
			}
			return GetOrAddComponent<ObservableCancelTrigger>(component.gameObject).OnCancelAsObservable();
		}

		public static UniRx.IObservable<PointerEventData> OnScrollAsObservable(this UIBehaviour component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<PointerEventData>();
			}
			return GetOrAddComponent<ObservableScrollTrigger>(component.gameObject).OnScrollAsObservable();
		}

		public static UniRx.IObservable<GameObject> OnParticleCollisionAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<GameObject>();
			}
			return GetOrAddComponent<ObservableParticleTrigger>(component.gameObject).OnParticleCollisionAsObservable();
		}

		public static UniRx.IObservable<Unit> OnParticleTriggerAsObservable(this Component component)
		{
			if (component == null || component.gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableParticleTrigger>(component.gameObject).OnParticleTriggerAsObservable();
		}

		public static UniRx.IObservable<int> OnAnimatorIKAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<int>();
			}
			return GetOrAddComponent<ObservableAnimatorTrigger>(gameObject).OnAnimatorIKAsObservable();
		}

		public static UniRx.IObservable<Unit> OnAnimatorMoveAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableAnimatorTrigger>(gameObject).OnAnimatorMoveAsObservable();
		}

		public static UniRx.IObservable<Collision2D> OnCollisionEnter2DAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Collision2D>();
			}
			return GetOrAddComponent<ObservableCollision2DTrigger>(gameObject).OnCollisionEnter2DAsObservable();
		}

		public static UniRx.IObservable<Collision2D> OnCollisionExit2DAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Collision2D>();
			}
			return GetOrAddComponent<ObservableCollision2DTrigger>(gameObject).OnCollisionExit2DAsObservable();
		}

		public static UniRx.IObservable<Collision2D> OnCollisionStay2DAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Collision2D>();
			}
			return GetOrAddComponent<ObservableCollision2DTrigger>(gameObject).OnCollisionStay2DAsObservable();
		}

		public static UniRx.IObservable<Collision> OnCollisionEnterAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Collision>();
			}
			return GetOrAddComponent<ObservableCollisionTrigger>(gameObject).OnCollisionEnterAsObservable();
		}

		public static UniRx.IObservable<Collision> OnCollisionExitAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Collision>();
			}
			return GetOrAddComponent<ObservableCollisionTrigger>(gameObject).OnCollisionExitAsObservable();
		}

		public static UniRx.IObservable<Collision> OnCollisionStayAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Collision>();
			}
			return GetOrAddComponent<ObservableCollisionTrigger>(gameObject).OnCollisionStayAsObservable();
		}

		public static UniRx.IObservable<Unit> OnDestroyAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Return(Unit.Default);
			}
			return GetOrAddComponent<ObservableDestroyTrigger>(gameObject).OnDestroyAsObservable();
		}

		public static UniRx.IObservable<Unit> OnEnableAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableEnableTrigger>(gameObject).OnEnableAsObservable();
		}

		public static UniRx.IObservable<Unit> OnDisableAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableEnableTrigger>(gameObject).OnDisableAsObservable();
		}

		public static UniRx.IObservable<Unit> FixedUpdateAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableFixedUpdateTrigger>(gameObject).FixedUpdateAsObservable();
		}

		public static UniRx.IObservable<Unit> LateUpdateAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableLateUpdateTrigger>(gameObject).LateUpdateAsObservable();
		}

		public static UniRx.IObservable<Collider2D> OnTriggerEnter2DAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Collider2D>();
			}
			return GetOrAddComponent<ObservableTrigger2DTrigger>(gameObject).OnTriggerEnter2DAsObservable();
		}

		public static UniRx.IObservable<Collider2D> OnTriggerExit2DAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Collider2D>();
			}
			return GetOrAddComponent<ObservableTrigger2DTrigger>(gameObject).OnTriggerExit2DAsObservable();
		}

		public static UniRx.IObservable<Collider2D> OnTriggerStay2DAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Collider2D>();
			}
			return GetOrAddComponent<ObservableTrigger2DTrigger>(gameObject).OnTriggerStay2DAsObservable();
		}

		public static UniRx.IObservable<Collider> OnTriggerEnterAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Collider>();
			}
			return GetOrAddComponent<ObservableTriggerTrigger>(gameObject).OnTriggerEnterAsObservable();
		}

		public static UniRx.IObservable<Collider> OnTriggerExitAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Collider>();
			}
			return GetOrAddComponent<ObservableTriggerTrigger>(gameObject).OnTriggerExitAsObservable();
		}

		public static UniRx.IObservable<Collider> OnTriggerStayAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Collider>();
			}
			return GetOrAddComponent<ObservableTriggerTrigger>(gameObject).OnTriggerStayAsObservable();
		}

		public static UniRx.IObservable<Unit> UpdateAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableUpdateTrigger>(gameObject).UpdateAsObservable();
		}

		public static UniRx.IObservable<Unit> OnBecameInvisibleAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableVisibleTrigger>(gameObject).OnBecameInvisibleAsObservable();
		}

		public static UniRx.IObservable<Unit> OnBecameVisibleAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableVisibleTrigger>(gameObject).OnBecameVisibleAsObservable();
		}

		public static UniRx.IObservable<Unit> OnBeforeTransformParentChangedAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableTransformChangedTrigger>(gameObject).OnBeforeTransformParentChangedAsObservable();
		}

		public static UniRx.IObservable<Unit> OnTransformParentChangedAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableTransformChangedTrigger>(gameObject).OnTransformParentChangedAsObservable();
		}

		public static UniRx.IObservable<Unit> OnTransformChildrenChangedAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableTransformChangedTrigger>(gameObject).OnTransformChildrenChangedAsObservable();
		}

		public static UniRx.IObservable<Unit> OnCanvasGroupChangedAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableCanvasGroupChangedTrigger>(gameObject).OnCanvasGroupChangedAsObservable();
		}

		public static UniRx.IObservable<Unit> OnRectTransformDimensionsChangeAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableRectTransformTrigger>(gameObject).OnRectTransformDimensionsChangeAsObservable();
		}

		public static UniRx.IObservable<Unit> OnRectTransformRemovedAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableRectTransformTrigger>(gameObject).OnRectTransformRemovedAsObservable();
		}

		public static UniRx.IObservable<GameObject> OnParticleCollisionAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<GameObject>();
			}
			return GetOrAddComponent<ObservableParticleTrigger>(gameObject).OnParticleCollisionAsObservable();
		}

		public static UniRx.IObservable<Unit> OnParticleTriggerAsObservable(this GameObject gameObject)
		{
			if (gameObject == null)
			{
				return Observable.Empty<Unit>();
			}
			return GetOrAddComponent<ObservableParticleTrigger>(gameObject).OnParticleTriggerAsObservable();
		}

		private static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
		{
			T val = gameObject.GetComponent<T>();
			if ((Object)val == (Object)null)
			{
				val = gameObject.AddComponent<T>();
			}
			return val;
		}
	}
}
