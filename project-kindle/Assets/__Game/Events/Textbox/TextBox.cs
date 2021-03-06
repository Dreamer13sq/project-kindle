using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBox : MasterObject
{
    // ====================================================================

    // Variables
    [SerializeField] private UnityEngine.UI.Image boxsprite;
    [SerializeField] private UnityEngine.UI.Image arrowsprite;
    [SerializeField] private RectTransform boxrect;
    [SerializeField] private UnityEngine.UI.Text textcomponent;

    enum State
    {
        zero,
        open,
        print,
        wait,
        close
    }
    
    private State state;
    private float statestep;
    private const float maxwidth = 700.0f;
    private const float maxheight = 100.0f;

    [SerializeField] string textstring;
    private string textstringshow;
    private float textpos;
    private float textspeed;

    private float arrowxstart;

    private int ticks;

    // ====================================================================

    // Functions

    // Start is called before the first frame update
    void Awake()
    {
        textstringshow = "";
        textpos = 0.0f;
        textspeed = 1.0f;
        textcomponent.text = "";

        boxrect.sizeDelta = new Vector2(0.0f, 0.0f);
        arrowxstart = arrowsprite.transform.localPosition.x;
        arrowsprite.enabled = false;

        statestep = 0.0f;
        state = State.zero;

        ticks = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            // ------------------------------------------------------------------
            case(State.open):   // Text box grows into size. Moves to "print" state when done
            {
                const float statetime = 10.0f;
                arrowsprite.enabled = false;
                textcomponent.enabled = false;

                // Update rectangle size
                if (statestep < statetime)
                {
                    statestep += 1.0f;
                    boxrect.sizeDelta = new Vector2(
                        maxwidth*(statestep/statetime),
                        maxheight*(statestep/statetime)
                    );
                }
                // Move to "print" state
                else
                {
                    statestep = 0.0f;
                    state = State.print;
                }
            }
            break;

            // ------------------------------------------------------------------
            case(State.print):  // Text is updated. Moves to "wait" state when done
            {
                arrowsprite.enabled = false;
                textcomponent.enabled = true;

                //Play printing sound while we are printing the text
                if(ticks == 0)
                {
                    game.PlaySound("TextPrint");
                }

                else if(ticks == 3)
                {
                    ticks = -1;
                }

                // Increase text position every frame
                if (textpos < textstring.Length)
                {
                    float tspd = textspeed;

                    // Increase print speed if FIRE is held
                    if ( Input.GetButton("Fire1") || Input.GetButton("Fire2") )
                    {
                        tspd *= 5.0f;
                    }

                    // Skip text if FIRE and MENU are held
                    if ( ( Input.GetButton("Fire1") || Input.GetButton("Fire2") ) && Input.GetButton("Menu") )
                    {
                        tspd = textstring.Length;
                    }

                    textpos = Mathf.Min(textpos + tspd, textstring.Length);
                    textstringshow = textstring.Substring(0, (int)textpos);
                    textcomponent.text = textstringshow;
                }
                // Move to "wait" state
                else
                {
                    statestep = 0.0f;
                    state = State.wait;

                    game.ContinueEvent();
                }
                ticks += 1;
            }
            break;

            // ------------------------------------------------------------------
            case(State.wait):   // Idle state
            {
                statestep += 0.1f;
                arrowsprite.enabled = true;
                textcomponent.enabled = true;
                ticks = 0;
                game.StopSound("TextPrint");

                // Arrow points right
                arrowsprite.transform.localPosition = new Vector2(
                    arrowxstart + Mathf.Sin(statestep)*4.0f,
                    arrowsprite.transform.localPosition.y
                    );
            }
            break;

            // ------------------------------------------------------------------
            case(State.close):  // Text box shrinks
            {
                const float statetime = 10.0f;
                arrowsprite.enabled = false;
                textcomponent.enabled = false;

                // Update rectangle size
                if (statestep < statetime)
                {
                    statestep += 1.0f;
                    boxrect.sizeDelta = new Vector2(
                        maxwidth*(1.0f-statestep/statetime),
                        maxheight*(1.0f-statestep/statetime)
                    );
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            break;
        }
    }

    public void AddText(string text)
    {
        textstring += text;
        statestep = 0.0f;
        state = (state == State.wait)? State.print: State.open;
    }

    public void ClearText()
    {
        textstring = "";
        textstringshow = "";
        textcomponent.text = "";
        textpos = 0.0f;
    }

    public void SetSpeed(float characterssperframe)
    {
        textspeed = characterssperframe;
    }

    public void Open()
    {
        statestep = 0.0f;
        state = State.open;
    }

    public void Print()
    {
        statestep = 0.0f;
        state = State.print;
    }

    public void Close()
    {
        statestep = 0.0f;
        state = State.close;
    }
}
