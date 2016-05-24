using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PhaseJumpUI : MonoBehaviour
{
    private Renderer gemRenderer;

    private PhaseJump jump;
    private GameObject staff;
    private GameObject effects;
    private GameObject gem;

    
    public Material standaredMaterial;
    public Material glowMaterial;
    public Material cooldownMaterial;

    private Text phaseForwardText;
    private Text phasebackwardText;

    // Use this for initialization
    void Start()
    {
        jump = GetComponent<PhaseJump>();

        GameObject Model = gameObject.transform.FindChild("Model").gameObject;
        if (Model != null)
        {
            staff = Model.transform.FindChild("Staff_Nim").gameObject;
            if (staff != null)
            {
                effects = staff.transform.FindChild("Effects").gameObject;
                gem = staff.transform.FindChild("MagTealGem").gameObject;
                gemRenderer = gem.GetComponent<Renderer>();
            }
        }

        // Get the text off the canvas
        phaseForwardText = FindObjectOfType<Canvas>().transform.FindChild("PhaseForwardText").gameObject.GetComponent<Text>();
        phasebackwardText = FindObjectOfType<Canvas>().transform.FindChild("PhaseBackText").gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (jump == null)
        {
            Debug.LogError("No PhaseJump script given.");
            return;
        }

        // Show nims phasing status as to if he can phase or not
        if (jump.isCoolingDown())
        {
            GrowStuff(true);
        }
        else if (!jump.isPhasing() && jump.canPhaseForward())
        {
            GrowStuff(true);
        }
        else if (!jump.isPhasing() && jump.canPhaseBack())
        {
            GrowStuff(true);
        }
        else
        {
            GrowStuff(false);
        }

        // Show GUI text
        bool canPhaseForward = jump.canPhaseForward();
        if( !jump.isPhasing() && canPhaseForward)
        {
            phaseForwardText.enabled = true;
        }
        else
        {
            phaseForwardText.enabled = false;
        }

        bool canPhaseBackward = jump.canPhaseBack();
        if (!jump.isPhasing() && canPhaseBackward)
        {
            phasebackwardText.enabled = true;
        }
        else
        {
            phasebackwardText.enabled = false;
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
        gemRenderer = gem.GetComponent<Renderer>();
        if (gemRenderer != null)
        {

            if (!shouldGlow)
            {
                gemRenderer.material = standaredMaterial; 
            }
            else if ( jump.isCoolingDown())
            {
                gemRenderer.material = cooldownMaterial;
            }
            else
            {
                gemRenderer.material = glowMaterial;
            }
        }
        else
        {
            Debug.Log("No Renderer for gem");
        }
    }
}

