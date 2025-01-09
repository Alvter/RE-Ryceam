using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class NoteMgr : MonoBehaviour
{
    private Note note;

    private int line;
    private int num;

    public void Init(Note note, int line, int num)
    {
        this.note = note;
        this.line = line;
        this.num = num;
        Timer.Register(note.et - Updater.Instance.realTime,() =>{
            switch (note.type)
            {
            case 0:
                GameController.Instance.Global.TapPool.Release(gameObject);
            break;
            case 1:
                GameController.Instance.Global.DragPool.Release(gameObject);
            break;
            case 2:
                GameController.Instance.Global.DFlickPool.Release(gameObject);
            break;
            case 3:
                GameController.Instance.Global.HoldPool.Release(gameObject);
            break;
            case 4:
                GameController.Instance.Global.FlickPool.Release(gameObject);
            break;
            }
            Destroy(this);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (note.type == 3) MoveHold();
        else MoveNote();
        // float scale = 1/ transform.parent.localScale.y;
    }

    void MoveNote()
    {
        transform.localPosition = new Vector3(0, Function.Instance.noteYpos(line, note.st), 0 );
    }

    void MoveHold()
    {
        transform.localPosition = new Vector3(0, Function.Instance.noteYpos(line, note.st), 0 );

        if (note.st < Updater.Instance.realTime) 
        {
            transform.localPosition = Vector3.zero;
            float holdLenght = Function.Instance.noteYpos(line, note.et) - Function.Instance.noteYpos(line, note.st);
            transform.GetChild(0).localScale = new Vector3(1, holdLenght / Global.Instance.holdHeight, 1);
        }
        else
        {
            float holdLenght = Function.Instance.noteYpos(line, note.et) - Function.Instance.noteYpos(line, note.st);
            transform.GetChild(0).localScale = new Vector3(1, holdLenght / Global.Instance.holdHeight, 1);
        }
    }
}
