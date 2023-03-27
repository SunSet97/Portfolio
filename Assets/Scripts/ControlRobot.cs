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
    private Animator RobotAnim;
    [Range(0,1)]public float Dissolve;
    [SerializeField]private float shader1;
    //[SerializeField]private float shader2;
    // Start is called before the first frame update
    void Start()
    {
        RobotAnim= Robot.GetComponent<Animator>();
        shaderLength=_Mshaders.Length;
        shader1=_Mshaders[0].GetComponent<Renderer>().material.GetFloat("_Cut");
    
    }

    // Update is called once per frame
    void Update()
    {
        changeDissolve(Dissolve);
    }

    public void changeDissolve(float value)
    {
        for(int i= 0; i<shaderLength;i++)
        {
            _Mshaders[i].GetComponent<Renderer>().material.SetFloat("_Cut",value);
            Debug.Log(_Mshaders[i].GetComponent<Renderer>().material.GetFloat("_Cut"));
        }

    }

    public void _attackTrigger()
    {
        RobotAnim.SetTrigger("Attack");
    }
}
