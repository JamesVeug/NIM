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

                Transform lightO = effects.transform.FindChild("Light");
                if (lightO != null)
                {
                    light = lightO.gameObject.GetComponent<Light>();
                }
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
        if(glowImage!= null )glowImage.enabled = glowTime > 0;
        if (jump.isPhasing())
        {
            if (glowImage != null)
            {
                glowImage.enabled = false;
                glowImage.transform.localScale = new Vector3(0, 0, 0);
            }

            glowTime = 0f;

            if (glowImage2 != null)
            {
                glowImage2.enabled = false;
            }
        }
        else
        {
            // We are not phasing, so show the glow
            if (glowImage2 != null) {
                glowImage2.enabled = true;
            }
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

        // Move the position
        // Change the color
        // HACK HACK HACK HACK HACK HACK
        Vector3 screenPos = Camera.main.WorldToScreenPoint(gem.transform.position)+new Vector3(0,1,0);
        if (glowImage != null) glowImage.transform.position = screenPos;
        if (glowImage2 != null) glowImage2.transform.position = screenPos;
        if (glowImage != null)
        {
            Color c = glowImage.color;
            c.a = glowTime / 2;
            glowImage.color = c;
        }


        // Scale the glow
        float scale = (1 - glowTime) * 5;
        if (glowImage != null) glowImage.transform.localScale = new Vector3(scale, scale, scale);


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
                if (glowImage2 != null)
                {
                    Color col = glowImage2.color;
                    col.a = 0;
                    if (light != null) light.color = col;
                    glowImage2.color = col;
                }
            }
            else if ( jump.isCoolingDown())
            {
                // Cooling down
                gemRenderer.material = cooldownMaterial;
                if (glowImage2 != null)
                {
                    Color col = glowImage2.color;
                    col.g = 0;
                    col.b = 0;
                    col.a = 0.5f;
                    glowImage2.color = col;
                    if (light != null) light.color = Color.red;
                }
            }
            else
            {
                // Glowing 
                gemRenderer.material = glowMaterial;
                if (glowImage2 != null)
                {
                    Color col = glowImage2.color;
                    col.g = 255;
                    col.b = 255;
                    col.a = 0.5f;
                    glowImage2.color = col;
                }

                // Make the staff glow white
                if (light != null) light.color = Color.white;

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

