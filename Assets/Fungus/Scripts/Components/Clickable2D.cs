// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Fungus
{
    /// <summary>
    /// Detects mouse clicks and touches on a Game Object, and sends an event to all Flowchart event handlers in the scene.
    /// The Game Object must have a Collider or Collider2D component attached.
    /// Use in conjunction with the ObjectClicked Flowchart event handler.
    /// </summary>
    public class Clickable2D : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("Is object clicking enabled")]
        [SerializeField] protected bool clickEnabled = true;

        [Tooltip("Mouse texture to use when hovering mouse over object")]
        [SerializeField] protected Texture2D hoverCursor;

        [Tooltip("Use the UI Event System to check for clicks. Clicks that hit an overlapping UI object will be ignored. Camera must have a PhysicsRaycaster component, or a Physics2DRaycaster for 2D colliders.")]
        [SerializeField] protected bool useEventSystem;

        [Tooltip("CUSTOM CODE: Change the hotspot of the mouse cursor.")]
        [SerializeField] protected Vector2 normalizedHotspot = new Vector2(0.0f,0.0f);
        public UnityEvent onMouseEnter;
        public UnityEvent onMouseExit;
        public UnityEvent onMouseClick;
        protected virtual void ChangeCursor(Texture2D cursorTexture)
        {
            if (!clickEnabled)
            {
                return;
            }
            
            if(cursorTexture == null){
                return;
            }
            Vector2 hotspot = normalizedHotspot;
            hotspot.x *= cursorTexture.width;
            hotspot.y *= cursorTexture.height;
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto); //MODIFIED: hotspot is changeable
        }

        protected virtual void DoPointerClick()
        {
            if (!clickEnabled)
            {
                return;
            }

            var eventDispatcher = FungusManager.Instance.EventDispatcher;

            eventDispatcher.Raise(new ObjectClicked.ObjectClickedEvent(this));
        }

        protected virtual void DoPointerEnter()
        {
            ChangeCursor(hoverCursor);
            if(onMouseEnter != null) onMouseEnter.Invoke();
        }

        protected virtual void DoPointerExit()
        {
            // Always reset the mouse cursor to be on the safe side
            SetMouseCursor.ResetMouseCursor();
            if(onMouseExit != null) onMouseExit.Invoke();
        }

        void Update()
        {
            //disable clicks when mixing
            // if (tag != "Immune")
            // {
            //     clickEnabled =  !GameManager.gm.notebookOpen && !GameManager.gm.UIOpen;
            // }
            
        }

        #region Legacy OnMouseX methods

        protected virtual void OnMouseDown()
        {
            if (!useEventSystem)
            {
                DoPointerClick();
            }
        }

        protected virtual void OnMouseEnter()
        {
            if (!useEventSystem)
            {
                DoPointerEnter();
            }
        }

        protected virtual void OnMouseExit()
        {
            if (!useEventSystem)
            {
                DoPointerExit();
            }
        }

        #endregion

        #region Public members

        /// <summary>
        /// Is object clicking enabled.
        /// </summary>
        public bool ClickEnabled { set { clickEnabled = value; } }

        #endregion

        #region IPointerClickHandler implementation

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log(gameObject.name + ":" + eventData.pointerPress);
            
            if (useEventSystem && eventData.pointerPress == gameObject)
            {
                if(eventData.button == PointerEventData.InputButton.Left) DoPointerClick();
            }
        }

        #endregion

        #region IPointerEnterHandler implementation

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log(gameObject.name + ":" + eventData.pointerEnter);
            if (useEventSystem && eventData.pointerEnter == gameObject)
            {
                DoPointerEnter();
            }
        }

        #endregion

        #region IPointerExitHandler implementation

        public void OnPointerExit(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                DoPointerExit();
            }
        }

        #endregion
    }
}
