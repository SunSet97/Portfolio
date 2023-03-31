using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ControlRobot : MonoBehaviour
{
    public GameObject Robot;
    //public Material[] _MShaders;
    public GameObject[] _Mshaders;
    public int shaderLength;
    public float coolTime=10f;
    public bool Power=false;
    private Animator RobotAnim;
    [SerializeField] private ParticleSystem fireVFX;
    [SerializeField] private ParticleSystem smokeVFX;
    // [SerializeField] private Animator fireAnimator;
    // [SerializeField] private Animator smokeAnimator;
    [Range(0,1)]public float Dissolve;
    void Start()
    {
        RobotAnim= Robot.GetComponent<Animator>();
        // fireAnimator=Robot.GetComponent<Animator>();
        // smokeAnimator=Robot.GetComponent<Animator>();
        shaderLength=_Mshaders.Length;
    
    }

    // Update is called once per frame
    void Update()
    {
        //changeDissolve(Dissolve);
    }

    public void changeDissolve(float value)
    {
        for(int i= 0; i<shaderLength;i++)
        {
            _Mshaders[i].GetComponent<Renderer>().material.SetFloat("_Cut",value);
            //Debug.Log(_Mshaders[i].GetComponent<Renderer>().material.GetFloat("_Cut"));
        }

    }
    public void PowerUp()
    {
        RobotAnim.SetTrigger("PowerUp");
        fireVFX.Play();
        // fireAnimator.SetTrigger("PowerUp");
        Debug.Log("Power UP!");
        StartCoroutine(dissolveShaderplus());
        
    }
        public void _attackTrigger()
    {
        RobotAnim.SetTrigger("Attack");
    }
    IEnumerator dissolveShaderplus()
    {
        Debug.Log("Start Dissolve Plus");
        float fixTime=Time.unscaledTime;
        float _time=0f;
        float value=0;
        if(Power)
        {
            while(_time<2f)
            {
                _time+=0.05f;
                value=_time/2;
                for(int i= 0; i<shaderLength;i++)
                {
                    //Debug.Log("Shader"+i+"value:"+value);
                    Debug.Log("Shader Dissolve"+_Mshaders[i].GetComponent<Renderer>().material.GetFloat("_Cut"));
                    _Mshaders[i].GetComponent<Renderer>().material.SetFloat("_Cut",value);
                }
                yield return null;
            }
        }
        yield return new WaitForSeconds(10f);
        StartCoroutine(dissolveShaderDiminish(Power));
    }

    IEnumerator dissolveShaderDiminish(bool Power)
    {
        Power=false;
        Debug.Log("Start Dissolve Diminish");
        float _time=2f;
        float value=1;
        smokeVFX.Play();
        // smokeAnimator.SetBool("Power",Power);
        if(Power==false)
        {   
            while(_time>0f)
            {
                _time-=0.01f;
                value=_time/2;
                for(int i= 0; i<shaderLength;i++)
                {
                    _Mshaders[i].GetComponent<Renderer>().material.SetFloat("_Cut",value);
                }
                yield return null;
            }    
        }
        
        

    }


}
