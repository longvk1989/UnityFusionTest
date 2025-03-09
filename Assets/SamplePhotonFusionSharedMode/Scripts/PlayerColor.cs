using Fusion;
using UnityEngine;

public class PlayerColor : NetworkBehaviour
{
    [SerializeField] private MeshRenderer _mesh;

    [Networked, OnChangedRender(nameof(ColorChanged))] 
    public Color MeshColor { get; set; }

    void Update()
    {
        if (HasStateAuthority && Input.GetKeyDown(KeyCode.E))
        {
            // Changing the material color here directly does not work since this code is only executed on the client pressing the button and not on every client.
            MeshColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }
    }

    private void ColorChanged()
    {
        _mesh.material.color = MeshColor;
    }
}
