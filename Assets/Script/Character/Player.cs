using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using static GridManager;

public class Player : MonoBehaviour
{

    //グリッド情報
    public int gridX;
    public int gridY;

    //入力処理
    public Vector2Int lastDirection;//最後の入力方向

    //リセット用
    private int initGridX;
    private int initGridY;

    //スプライト
    private SpriteRenderer spr;
    public Sprite[] shotPlayer;
    private Animator animator;
    private CharacterVisual visual;
    private bool lastShotFlipX = false;//最後に撃った左右の方向
    private bool wasShotHorizontal = false;//左右に撃ったか

    //ショット
    private float shotNum;//左右1,上2,下3

    //SE
    private AudioSource audioSource;
    public AudioClip moveClip;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        visual = GetComponent<CharacterVisual>();
    }
    void Start()
    {
        TurnManager.Instance.SetPlayer(gameObject.GetComponent<Player>());
        SetPos();
        //リセット用
        initGridX = gridX;
        initGridY = gridY;
        SnapToGrid();
    }
    void Update()
    {
        PlayerInput();
        ArrowInput();
        ChangeSprite();
    }

    //座標の設定
    void SnapToGrid() { transform.position = GridManager.Instance.GridToWorld(gridX, gridY); }
    //移動入力処理
    public void PlayerInput()
    {
        Vector2Int dir = Vector2Int.zero;

        //入力
        if(Gamepad.current != null)
        {
            //斜め入力防止
            Vector2 stickInput = Gamepad.current.leftStick.ReadValue();
            float stickValue = 0.5f;

            //stickValue以上入力が掛かったら
            if (stickInput.magnitude >= stickValue)
            {
                if(Mathf.Abs(stickInput.x) > Mathf.Abs(stickInput.y))//左右値が強かった場合
                {
                    if (stickInput.x > 0 && Gamepad.current.leftStick.right.wasPressedThisFrame) dir = Vector2Int.right;
                    else if (stickInput.x < 0 && Gamepad.current.leftStick.left.wasPressedThisFrame) dir = Vector2Int.left;
                }
                else//上下値が強かった場合
                {
                    if (stickInput.y > 0 && Gamepad.current.leftStick.up.wasPressedThisFrame) dir = Vector2Int.up;
                    else if (stickInput.y < 0 && Gamepad.current.leftStick.down.wasPressedThisFrame) dir = Vector2Int.down;
                }
            }
        }
        else
        {
            if (Keyboard.current.wKey.wasPressedThisFrame) { dir = Vector2Int.up; }
            if (Keyboard.current.aKey.wasPressedThisFrame) { dir = Vector2Int.left; }
            if (Keyboard.current.sKey.wasPressedThisFrame) { dir = Vector2Int.down; }
            if (Keyboard.current.dKey.wasPressedThisFrame) { dir = Vector2Int.right; }
        }

        //directionが変わったら通知
        if(dir != Vector2Int.zero)
        {
            TurnManager.Instance.isPlayerInput(dir);
        }
    }
    //左右上下のセルに移動可能かを取得
    public bool CanMove(Vector2Int direction)
    {
        //セル情報の取得
        int targetX = gridX + direction.x;
        int targetY = gridY + direction.y;
        Cell targetCell = GridManager.Instance.GetCell(targetX, targetY);

        if (targetCell == null || !targetCell.isWalk) return false;
        return true;
    }
    //移動方向の保存
    public void SetMoveDirection(Vector2Int direction) { lastDirection = direction; }
    //移動の実行
    public void PlayerMove()
    {
        int targetX = gridX + lastDirection.x;
        int targetY = gridY + lastDirection.y;

        //再度判定を取る（ダメージ判定のない壁になる敵が出てきた時用）
        var targetCell = GridManager.Instance.GetCell(targetX, targetY);
        if (targetCell == null || !targetCell.isWalk)
        {
            MoveCancel(targetCell);
            return;
        }

        //位置変更
        gridX = targetX;
        gridY = targetY;
        SnapToGrid();

        //方向にあわせてスプライトを変更
        if (lastDirection.x != 0)spr.flipX = lastDirection.x > 0;

        //SE
        audioSource.PlayOneShot(moveClip);

        //移動アニメーション
        visual?.PlayMoveStretch(lastDirection);

    }
    //エネミー実行後に移動をキャンセルする場合
    private void MoveCancel(GridManager.Cell cancelCell)
    {
        //ダメージ処理などの分岐は後で書く
        Debug.Log("移動がキャンセルされました");
    }
    //位置を綺麗に修正
    void SetPos()
    {
        Vector2Int pos = GridManager.Instance.WorldToGrid(transform.position);
        gridX = pos.x;
        gridY = pos.y;
    }
    //リセット
    public void PlayerReset()
    {
        gridX = initGridX;
        gridY = initGridY;
        lastDirection = Vector2Int.zero;
        SnapToGrid();
    }
    //矢の発射
    public void ArrowInput()
    {
        Vector2Int dir = Vector2Int.zero;

        if(Gamepad.current != null)
        {
            //コントローラー
            if (Gamepad.current.buttonNorth.wasPressedThisFrame) dir = Vector2Int.up;
            if (Gamepad.current.buttonSouth.wasPressedThisFrame) dir = Vector2Int.down;
            if (Gamepad.current.buttonWest.wasPressedThisFrame) dir = Vector2Int.left;
            if (Gamepad.current.buttonEast.wasPressedThisFrame) dir = Vector2Int.right;
        }
        else
        {
            //キーボード
            if (Keyboard.current.upArrowKey.wasPressedThisFrame) dir = Vector2Int.up;
            if (Keyboard.current.downArrowKey.wasPressedThisFrame) dir = Vector2Int.down;
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame) dir = Vector2Int.left;
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame) dir = Vector2Int.right;
        }
        if (dir != Vector2Int.zero)
        {
            //スプライト用にshotNumを変更
            if (dir == Vector2Int.left)
            {
                shotNum = 1;
                lastShotFlipX = false;
                wasShotHorizontal = true;
            }
            if(dir == Vector2Int.right)
            {
                shotNum = 1; 
                lastShotFlipX = true;
                wasShotHorizontal = true;
            }
            if (dir == Vector2Int.up) shotNum = 2;
            if (dir == Vector2Int.down) shotNum = 3;
            //ショット
            TurnManager.Instance.FireArrow(dir); 
        }

    }
    //リスポーン位置の設定
    public void SetInitPos(Vector2Int vec)
    {
        initGridX = vec.x;
        initGridY = vec.y;
    }
    //スプライト管理
    public void ChangeSprite()
    {
        //ショット時にスプライト変更
        if(TurnManager.Instance.turnState == TurnManager.TurnState.Arrow)
        {
            //アニメーターの停止
            animator.enabled = false;
            //スプライトの変更
            for (int i = 0; i < 3; i++) 
            {
                //左右に撃ったら撃った方向を向く
                if (wasShotHorizontal) spr.flipX = lastShotFlipX;
                if (shotNum == i+1) spr.sprite = shotPlayer[i];
            }
        }
        else animator.enabled = true;
    }
    //CharavterVisualの取得
    public CharacterVisual GetVisual() { return visual; }

}
