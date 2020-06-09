using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UIDrawable
{
    /// <summary>
    /// Component that makes a drawable surface using rendertextures and shaders
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class UIDrawable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Initialization")]
        [Tooltip("Initialization color of the drawing surface (also used for clearing and erasing)")]
        [SerializeField]
        private Color32 m_initializationColor;

        [Tooltip("Scale multiplier for the rendertexture, 1 meaning the width/height of this rect")]
        [SerializeField]
        [Range(0.5f, 4)]
        private float m_renderMultiplier = 1;

        [Tooltip("Helper list of selectable colours (note, one can also use: SetColor(Color32))")]
        [SerializeField]
        private Color32[] m_colors;

        [Header("Drawing settings")]
        [Tooltip("Thickness of the brush")]
        [SerializeField]
        private float m_thickness = 1;

        [Tooltip("Flow/strength of the brush")]
        [SerializeField]
        [Range(0,10f)]
        private float m_flow = 5;

        [Tooltip("Whether to apply color when drawing and stopping still")]
        [SerializeField]
        private bool m_drawStationary = true;

        [Tooltip("Lerp speed of the drawing")]
        [SerializeField]
        [Range(5,30f)]
        private float m_positionLerp = 20;

        private CustomRenderTexture m_customtex;
        private Vector3 lastpos = Vector3.zero;
        private RectTransform m_rect;
        private Color32 m_currentColor = Color.black;
        private bool m_pointerDownOnElement = false;
        private RawImage m_displayImage;
        private Material m_drawingMaterial;


        #region Public
        public void SetThickness(float value)
        {
            m_thickness = value;
        }

        public void SetColor(Color32 newcolor)
        {
            SetCurrentColor(newcolor);
        }

        public void SetColor(int index)
        {
            SetCurrentColor(m_colors[index]);
        }

        /// <summary>
        /// Changes to eraser. Erasing color being the initializationColor
        /// </summary>
        public void SetEraser()
        {
            SetCurrentColor(m_initializationColor);
        }

        /// <summary>
        /// Erases all content of the drawable surface
        /// </summary>
        public void EraseAll()
        {
            m_customtex.Initialize();
        }

        public CustomRenderTexture CurrentTexture
        {
            get
            {
                return m_customtex;
            }
        }

        public Color32 CurrentBrushColor
        {
            get
            {
                return m_currentColor;
            }
        }
        #endregion


        #region UnityCBs
        private void Awake()
        {
            m_displayImage = GetComponent<RawImage>();
            m_rect = GetComponent<RectTransform>();

            m_drawingMaterial = Resources.Load<Material>("SimpleDrawingMaterial");
            m_customtex = new CustomRenderTexture((int)(m_rect.rect.width * m_renderMultiplier), (int)(m_rect.rect.height * m_renderMultiplier));
            m_drawingMaterial = new Material(m_drawingMaterial);
            m_customtex.material = m_drawingMaterial;
            m_drawingMaterial.SetTexture("_Tex", m_customtex);
            m_drawingMaterial.SetFloat("_heightRatio", (float)m_customtex.height / (float)m_customtex.width);
            m_customtex.updateZoneSpace = CustomRenderTextureUpdateZoneSpace.Pixel;
            m_customtex.doubleBuffered = true;
            m_customtex.initializationMode = CustomRenderTextureUpdateMode.OnDemand;
            m_customtex.initializationColor = m_initializationColor;
            m_customtex.updateMode = CustomRenderTextureUpdateMode.OnDemand;
            m_customtex.Create();
            m_customtex.Initialize();
            m_displayImage.texture = m_customtex;

            if(m_colors.Length > 0)
                SetColor(0);

            // init lastpos / currentpos to some negative value out of the canvas
            lastpos = NormalizeToElementSpace(-Vector2.one);
            UpdateCursorPosition(-Vector2.one);
        }

        private void Update()
        {
            if (!m_pointerDownOnElement)
                return;

            UpdateCursorPosition(Input.mousePosition);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrawingStart(eventData.position);
            m_pointerDownOnElement = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_pointerDownOnElement = false;
            OnDrawingEnd(eventData.position);
        }
        #endregion

        #region Private
        private void SetCurrentColor(Color32 col)
        {
            m_currentColor = col;
        }

        private void OnDrawingStart(Vector2 position)
        {
            lastpos = NormalizeToElementSpace(position);
        }

        private void OnDrawingEnd(Vector2 position)
        {
            lastpos = NormalizeToElementSpace(position);
        }

        private void UpdateCursorPosition(Vector3 cursorPosition)
        {
            Vector3 mouseposition = NormalizeToElementSpace(cursorPosition);

            mouseposition = Vector3.Lerp(lastpos, mouseposition, Time.deltaTime * m_positionLerp);

            if(!m_drawStationary && (mouseposition - lastpos) == Vector3.zero)
            {
                return;
            }

            m_drawingMaterial.SetVector("_startpos", lastpos);
            m_drawingMaterial.SetVector("_endpos", mouseposition);
            m_drawingMaterial.SetFloat("_thickness", m_thickness);
            m_drawingMaterial.SetColor("_Color", m_currentColor);
            m_drawingMaterial.SetFloat("_flow", m_flow);

            lastpos = mouseposition;

            m_customtex.Update();
        }

        private Vector3 NormalizeToElementSpace(Vector3 position, bool normalize = true)
        {
            position = transform.InverseTransformPoint(position);

            position.x = position.x * m_renderMultiplier + (m_customtex.width / 2);
            position.y = position.y * m_renderMultiplier + (m_customtex.height / 2);

            // Rect pivot position fix
            position.x = position.x - m_customtex.width * (0.5f - m_rect.pivot.x);
            position.y = position.y - m_customtex.height * (0.5f - m_rect.pivot.y);

            if (normalize)
            {
                position.x /= (float)m_customtex.width;
                position.y /= (float)m_customtex.height;
            }

            return position;
        }

        #endregion
    }
}