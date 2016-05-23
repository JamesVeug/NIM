using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PhaseJumpUI : MonoBehaviour {
    private PhaseJump jump;
    private GameObject staff;
    private GameObject effects;
    private GameObject gem;

    public Material standaredMaterial;
    public Material glowMaterial;

    // Use this for initialization
    void Start () {
        jump = GetComponent<PhaseJump>();

        GameObject Model = gameObject.transform.FindChild("Model").gameObject;
        if (Model != null)
        {
            staff = Model.transform.FindChild("Staff_Nim").gameObject;
            if (staff != null)
            {
                effects = staff.transform.FindChild("Effects").gameObject;
                gem = staff.transform.FindChild("MagTealGem").gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
	    if( jump == null)
        {
            Debug.LogError("No PhaseJump script given.");
            return;
        }


        // Phase Forward Button
        if ( !jump.isPhasing() && jump.canPhaseForward() )
        {
            GrowStuff(true);
        }
        else if ( !jump.isPhasing() && jump.canPhaseBack())
        {
            GrowStuff(true);
        }
        else
        {
            GrowStuff(false);
        }
    }

    public void GrowStuff(bool shouldGlow)
    {
        if (effects == null)
        {
            Debug.Log("can not find child Effects in Model!");
            return;
        }

        // effects
        foreach (Transform child in effects.transform)
        {
            ParticleSystemRenderer r = child.gameObject.GetComponent<ParticleSystemRenderer>();
            if (r != null) { r.enabled = shouldGlow; }


            Light l = child.gameObject.GetComponent<Light>();
            if (l != null) { l.enabled = shouldGlow; }
        }
        

        // Gem glow
        Renderer gemRenderer = gem.GetComponent<Renderer>();
        if (gemRenderer != null)
        {
            
            if (shouldGlow)
            {
                gemRenderer.material = glowMaterial;
            }
            else
            {
                gemRenderer.material = standaredMaterial;
            }
        }
        else
        {
            Debug.Log("No Renderer for gem");
        }
    }
}

