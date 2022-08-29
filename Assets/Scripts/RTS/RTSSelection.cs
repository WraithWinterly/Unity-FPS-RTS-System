/*
 * 2022 WraithWinterly
 * I did not make this on my own.
 * https://www.youtube.com/watch?v=OL1QgwaDsqo
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RTSSelection : MonoBehaviour
{
    private RTSSelectedUnitsTable _selectedUnitsTable;
    private MeshCollider _selectionBox;
    private Mesh _selectionMesh;

    private RaycastHit _hit;

    private Vector3[] _verts;
    private Vector3[] _vecs;
    private Vector2[] _corners;

    private Vector3 _mousePosStart;
    private Vector3 _mousePosEnd;

    private float _selectionBoxColorAlpha = 0f;
    private float _selectionBorderColorAlpha = 0f;

    private bool _dragSelect;

    // Force new click if mouse if already down
    private bool _waitLeftClickUp;

    [SerializeField] private Camera rtsCamera;

    private void Awake()
    {
        _selectedUnitsTable = GetComponent<RTSSelectedUnitsTable>();
        _selectionBox = gameObject.AddComponent<MeshCollider>();
        _selectionBox.convex = true;
        _selectionBox.isTrigger = true;
        _dragSelect = false;
    }

    private void Start()
    {
        OnEnable();
    }

    private void OnEnable()
    {
        Refs.Inst.EventManager?.modeSwitchedRTS.AddListener(OnRTSMode);
    }

    private void OnRTSMode()
    {
        if (Input.GetButton(Const.shoot))
        {
            _waitLeftClickUp = true;
        }

    }

    private void Update()
    {
        if (RTSManager.FPSMode())
        {
            _selectedUnitsTable.RemoveAllDict();
            _selectedUnitsTable.UpdateSelections();
            _dragSelect = false;
            return;
        }

        if (_waitLeftClickUp)
        {
            if (Input.GetButtonUp(Const.shoot))
            {
                _waitLeftClickUp = false;
            }
            else
            {
                _selectedUnitsTable.RemoveAllDict();
                _selectedUnitsTable.UpdateSelections();
                _dragSelect = false;
                return;
            }
        }

        // Clicked
        if (Input.GetButtonDown(Const.shoot))
        {
            _mousePosStart = Input.mousePosition;
        }

        // While Held
        if (Input.GetButton(Const.shoot))
        {
            if ((_mousePosStart - Input.mousePosition).magnitude > 40)
            {
                _dragSelect = true;
            }
        }

        if (Input.GetButtonUp(Const.shoot))
        {
            if (!_dragSelect)
            {
                SelectUnitsWithClick();
            }
            else
            {
                SelectUnitsWithDrag();
            }

            _dragSelect = false;
        }
    }

    private void SelectUnitsWithClick()
    {
        Ray ray = rtsCamera.ScreenPointToRay(_mousePosStart);

        if (Physics.Raycast(ray, out _hit, 5000, Const.layerMaskFriendly))
        {
            // Shift click
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (_selectedUnitsTable.HasUnit(_hit.transform.gameObject))
                {
                    print("Deselect");
                    _selectedUnitsTable.RemoveFromDict(_hit.transform.gameObject);
                }
                else
                {
                    print("Shift Select - Add New");
                    _selectedUnitsTable.AddToDict(_hit.transform.gameObject);
                }
            }
            // Single click
            else
            {
                // Deselect
                if (_selectedUnitsTable.SelectedTable.Keys.Count == 1 && _selectedUnitsTable.HasUnit(_hit.transform.gameObject))
                {
                    _selectedUnitsTable.RemoveFromDict(_hit.transform.gameObject);
                }
                else
                {
                    _selectedUnitsTable.RemoveAllDict();
                    _selectedUnitsTable.AddToDict(_hit.transform.gameObject);
                }

            }
        }
        // Deselect by clicking off
        else
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                _selectedUnitsTable.RemoveAllDict();
            }
        }
        _selectedUnitsTable.UpdateSelections();
    }

    private void SelectUnitsWithDrag()
    {
        _verts = new Vector3[4];
        _vecs = new Vector3[4];

        int i = 0;
        int validCorners = 0;

        _mousePosEnd = Input.mousePosition;
        _corners = GetBoundingBox(_mousePosStart, _mousePosEnd);

        foreach (Vector2 corner in _corners)
        {
            Ray ray = rtsCamera.ScreenPointToRay(corner);

            if (Physics.Raycast(ray, out _hit, 50000, Const.layerMaskGround))
            {
                _verts[i] = new Vector3(_hit.point.x, 0, _hit.point.z);
                _vecs[i] = ray.origin - _hit.point;
                Debug.DrawLine(rtsCamera.ScreenToWorldPoint(corner), _hit.point, Color.red, 1.0f);
                validCorners++;
            }
            i++;
        }

        if (validCorners >= _corners.Length)
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                _selectedUnitsTable.RemoveAllDict();
            }

            _selectionMesh = GenerateSelectionMesh(_verts, _vecs);
            _selectionBox.enabled = true;
            _selectionBox.sharedMesh = _selectionMesh;


            StartCoroutine(DisableSelectionBox());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Const.layerFriendly)
        {
            _selectedUnitsTable.AddToDict(other.gameObject);
        }
    }

    private IEnumerator DisableSelectionBox()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        _selectedUnitsTable.UpdateSelections();
        _selectionBox.enabled = false;

    }

    private void OnGUI()
    {
        if (_dragSelect)
        {
            _selectionBoxColorAlpha = 0.25f;
            _selectionBorderColorAlpha = 0.95f;
        }
        else
        {
            _selectionBoxColorAlpha -= Time.deltaTime;
            _selectionBorderColorAlpha -= Time.deltaTime * (0.95f / 0.25f);
            _selectionBoxColorAlpha = Mathf.Clamp(_selectionBoxColorAlpha, 0, 1);
            _selectionBorderColorAlpha = Mathf.Clamp(_selectionBorderColorAlpha, 0, 1);
        }

        Rect rect;

        if (CanDrag())
        {
            _mousePosEnd = Input.mousePosition;
            rect = RTSSelectionDragBox.GetScreenRect(_mousePosStart, Input.mousePosition);
        }
        else
        {
            rect = RTSSelectionDragBox.GetScreenRect(_mousePosStart, _mousePosEnd);
        }

        RTSSelectionDragBox.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, _selectionBoxColorAlpha));
        RTSSelectionDragBox.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f, _selectionBorderColorAlpha));
    }

    private bool CanDrag()
    {
        return (RTSManager.RTSMode() && _dragSelect && !_waitLeftClickUp);
    }

    //Create a bounding box (4 corners in order) from the start and end mouse position
    private Vector2[] GetBoundingBox(Vector2 p1, Vector2 p2)
    {
        // Min and Max to get 2 corners of rectangle regardless of drag direction.
        Vector3 bottomLeft = Vector3.Min(p1, p2);
        Vector3 topRight = Vector3.Max(p1, p2);

        // 0 = top left; 1 = top right; 2 = bottom left; 3 = bottom right;
        Vector2[] corners =
        {
            new Vector2(bottomLeft.x, topRight.y),
            new Vector2(topRight.x, topRight.y),
            new Vector2(bottomLeft.x, bottomLeft.y),
            new Vector2(topRight.x, bottomLeft.y)
        };
        return corners;

    }

    // Generate a mesh from the 4 bottom points
    private Mesh GenerateSelectionMesh(IReadOnlyList<Vector3> corners, IReadOnlyList<Vector3> vecs)
    {
        var verts = new Vector3[8];

        // Map the tris of our cube
        int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 };

        for (int i = 0; i < 4; i++)
        {
            verts[i] = corners[i];
        }

        for (int j = 4; j < 8; j++)
        {
            verts[j] = corners[j - 4] + vecs[j - 4];
        }

        Mesh selectionMesh = new()
        {
            vertices = verts,
            triangles = tris
        };

        return selectionMesh;
    }
}
