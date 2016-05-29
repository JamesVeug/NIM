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
    private GameObject glowObject;


    public Material standaredMaterial;
    public Material glowMaterial;
    public Material cooldownMaterial;

    private Light light;
    private Text phaseForwardText;
    private Text phasebackwardText;
    private Image glowImage2;
    private Image glowImage;
    private float glowTime = 0f;
    private bool hasGlown = false;

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
                light = effects.transform.FindChild("Light").gameObject.GetComponent<Light>();
            }
        }

        // Get the text off the canvas
        Transform forwardText = FindObjectOfType<Canvas>().transform.FindChild("PhaseForwardText");
        if (forwardText != null) { phaseForwardText = forwardText.gameObject.GetComponent<Text>(); }

        Transform backText = FindObjectOfType<Canvas>().transform.FindChild("PhaseBackText");
        if (backText != null) { phasebackwardText = backText.gameObject.GetComponent<Text>(); }

        Transform glowText = FindObjectOfType<Canvas>().transform.FindChild("NimGlow");
        if (glowText != null) { glowImage = glowText.gameObject.GetComponent<Image>(); glowImage.enabled = false; }

        Transform glowText2 = FindObjectOfType<Canvas>().transform.FindChild("NimGlow2");
        if (glowText2 != null) { glowImage2 = glowText2.gameObject.GetComponent<Image>(); }
    }

    // Update is called once per frame
    void Update()
    {
        if (jump == null)
        {
            Debug.LogError("No PhaseJump script given.");
            return;
        }
        glowTime = Mathf.Max(0, glowTime - Time.deltaTime);
        glowImage.enabled = glowTime > 0;
        if( jump.isPhasing())
        {
            glowImage.enabled = false;
            glowImage.transform.localScale = new Vector3(0, 0, 0);
            glowTime = 0f;
            glowImage2.enabled = false;
        }
        else
        {
            glowImage2.enabled = true;
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
        if (phaseForwardText != null)
        {
            bool canPhaseForward = jump.canPhaseForward();
            if (!jump.isPhasing() && canPhaseForward)
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
        
    }

    public void GrowStuff(bool shouldGlow)
    {

        if (effects == null)
        {
            Debug.Log("can not find child Effects in Model!");
            return;
        }

        
        // Stop glowing
        if (!shouldGlow && hasGlown && glowTime == 0)
        {
            hasGlown = false;
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(gem.transform.position)+new Vector3(0,1,0);
        glowImage.transform.position = screenPos;
        glowImage2.transform.position = screenPos;
        Color c = glowImage.color;
        c.a = glowTime/2;
        glowImage.color = c;


        // Scale the glow
        float scale = (1 - glowTime) * 5;
        glowImage.transform.localScale = new Vector3(scale, scale, scale);


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
                // No glow
                gemRenderer.material = standaredMaterial;
                Color col = glowImage2.color;
                col.a = 0;
                light.color = col;
                glowImage2.color = col;
            }
            else if ( jump.isCoolingDown())
            {
                // Cooling down
                gemRenderer.material = cooldownMaterial;
                Color col = glowImage2.color;
                col.g = 0;
                col.b = 0;
                col.a = 0.5f;
                glowImage2.color = col;
                light.color = Color.red;
            }
            else
            {
                // Glowing 
                gemRenderer.material = glowMaterial;
                Color col = glowImage2.color;
                col.g = 255;
                col.b = 255;
                col.a = 0.5f;
                glowImage2.color = col;

                light.color = Color.white;

                if (shouldGlow && !hasGlown)
                {
                    glowTime = 1;
                    hasGlown = true;
                }
            }
        }
        else
        {
            Debug.Log("No Renderer for gem");
        }
    }
}

