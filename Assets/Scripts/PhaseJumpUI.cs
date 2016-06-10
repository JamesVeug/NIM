using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PhaseJumpUI : MonoBehaviour
{
    private Renderer gemRenderer;
    private AudioSource audioSource;
    private PhaseJump jump;

    private GameObject staff;
    private GameObject effects;
    private GameObject gem;
    private GameObject glowObject;
    public GameObject trail;
    public GameObject phaseIn;
    public GameObject phaseOut;

    private UnityStandardAssets.ImageEffects.DepthOfField dofScript;
    private ChasePlayer chaseScript;
    public GameObject marker;
    public GameObject PhaseLight;
    private bool isPreviewing;
    private bool isPhasing;


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

    // Phase Jumping
    private bool phaseButtonPressed = false;

    [Range(min: 0, max: 100)]
    public float[] PreviewSoundsVolume;

    // Clips
    public AudioClip[] PreviewSounds;


    // Use this for initialization
    void Start()
    {
        jump = GetComponent<PhaseJump>();
        audioSource = GetComponent<AudioSource>();


        Transform staffTransform = transform.FindChild("Staff_Nim");
        if(staffTransform != null)
        {
            staff = staffTransform.gameObject;
            effects = staff.transform.FindChild("Effects").gameObject;
            gem = staff.transform.FindChild("MagTealGem").gameObject;
            gemRenderer = gem.GetComponent<Renderer>();

            Transform lightO = effects.transform.FindChild("Light");
            if (lightO != null)
            {
                light = lightO.gameObject.GetComponent<Light>();
            }
        }

        
        dofScript = Camera.main.GetComponent<UnityStandardAssets.ImageEffects.DepthOfField>();
        chaseScript = Camera.main.GetComponent<ChasePlayer>();

        setPhaseLight(false);
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
    void LateUpdate()
    {
        if (jump == null)
        {
            Debug.LogError("No PhaseJump script given.");
            return;
        }

        // Update Nims glow on his staff
        updateGlow();

        // Update Nims preview Camera
        updatePhaseJump();
    }

    private void setMarkerVisibility(GameObject o, bool visible)
    {
        if (o == null)
        {
            return;
        }

        Renderer rend = o.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.enabled = visible;
        }
        Light light = o.GetComponent<Light>();
        if( light != null)
        {
            light.enabled = visible;
        }

        foreach (Transform t in o.transform)
        {
            setMarkerVisibility(t.gameObject, visible);
        }
    }

    private void disablePreviewCamera()
    {
        // Play sound if we have to move the camera a far distance
        if( (chaseScript.whatToChase.transform.position-gameObject.transform.position).magnitude > 1)
        {
            SoundMaster.playRandomSound(PreviewSounds, PreviewSoundsVolume, getAudioSource());
        }

        // Start chasing the player again
        if (chaseScript.whatToChase != gameObject)
        {
            chaseScript.whatToChase = gameObject;
        }

        // Hide the marker
        setMarkerVisibility(marker, false);
        isPreviewing = false;
    }


    private void updatePhaseJump()
    {
        float phaseJumpDirection = Input.GetAxis("PhaseJump");
        float preview = Input.GetAxis("PreviewPhase");

        // Effects for when we phase
        if( jump.isPhasing() && !isPhasing)
        {
            // Starting phasing
            isPhasing = true;
            setPhaseLight(isPhasing);
        }
        else if( !jump.isPhasing() && isPhasing)
        {
            // Finished phasing
            cloneParticle(phaseOut);
            isPhasing = false;
            setPhaseLight(isPhasing);
            disablePreviewCamera();
        }

        // Disable the camera if we release all buttons and are not phasing
        if (Mathf.Abs(preview) != 1 && isPreviewingPhase() && !jump.isPhasing()) {
            disablePreviewCamera();
        }
        
        // Preview
        if( preview == 1 && jump.canPhaseForward() && !isPreviewingPhase())
        {
            previewPhase(true);
        }
        else if (preview == -1 && jump.canPhaseBack() && !isPreviewingPhase())
        {
            previewPhase(false);
        }


        // Phase
        if (phaseJumpDirection == 1 && jump.canPhaseForward())
        {
            // Create trail
            cloneParticle(trail);
            cloneParticle(phaseIn);

            // Phase
            jump.phaseForward();
            setMarkerVisibility(marker, false);
        }
        else if (phaseJumpDirection == -1 && jump.canPhaseBack())
        {
            // Create trail
            cloneParticle(trail);
            cloneParticle(phaseIn);

            // Phase
            jump.phaseBack();
            setMarkerVisibility(marker, false);
        }


        // Focus on the player again
        if (dofScript != null)
        {
            dofScript.focalLength = (chaseScript.whatToChase.transform.position - Camera.main.transform.position).magnitude;
        }
    }

    private bool isPreviewingPhase()
    {
        return isPreviewing;
    }

    private void previewPhase(bool previewForward)
    {
        if( marker == null)
        {
            Debug.LogWarning("No Marker object assigned to NIM!");
            return;
        }
        

        MovementWaypoint newWaypoint;
        Vector3 newPos = marker.transform.position;
        jump.getPhasePoint(previewForward, out newPos, out newWaypoint);
        marker.transform.position = newPos;
        marker.transform.rotation = transform.rotation;

        // Preview back position
        chaseScript.whatToChase = marker;
        //dofScript.focalLength = (marker.transform.position - chaseScript.getNewPosition()).magnitude;

        SoundMaster.playRandomSound(PreviewSounds, PreviewSoundsVolume, getAudioSource());
        setMarkerVisibility(marker, true);
        isPreviewing = true;
    }

    private void updateGlow()
    {

        glowTime = Mathf.Max(0, glowTime - Time.deltaTime);
        if (glowImage != null) glowImage.enabled = glowTime > 0;
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
            if (glowImage2 != null)
            {
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
            Debug.Log("can not find child 'Effects' in Model!");
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
        if (glowImage2 != null)
        {
            glowImage2.GetComponent<Image>().enabled = Camera.main.transform.position.x > Camera.main.ScreenToWorldPoint(glowImage2.transform.position).x;
            glowImage2.transform.position = screenPos;
        }
        if (glowImage != null)
        {
            glowImage.GetComponent<Image>().enabled = Camera.main.transform.position.x > Camera.main.ScreenToWorldPoint(glowImage.transform.position).x;
            Color c = glowImage.color;
            glowImage.transform.position = screenPos;
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
                    setLightColor(Color.red);
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
                    setLightColor(col);
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

    private void setLightColor(Color col)
    {
        if( light != null)
        {
            light.color = col;
        }
    }

    public void cloneParticle(GameObject o)
    {
        if (o != null)
        {
            GameObject cloneParticle = (GameObject)Instantiate(o);
            cloneParticle.transform.position = gameObject.transform.position;
            cloneParticle.transform.parent = gameObject.transform;
            Destroy(cloneParticle, 2);
        }
    }

    public void setPhaseLight(bool active)
    {
        if( PhaseLight != null)
        {
            PhaseLight.SetActive(active);
        }
    }

    public AudioSource getAudioSource()
    {
        if (audioSource == null)
        {
            Debug.LogError("Object " + gameObject.name + " does not have an AudioSource Component!");
        }
        return audioSource;
    }
}

