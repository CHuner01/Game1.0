using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character1 : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField] 
    private float _jumpForce;

    [SerializeField]
    private SpriteRenderer _characterSprite;

    private Rigidbody2D _rigidbody;
    private bool _isGrounded;
    private Vector3 _input;
    private Animator _animator;
    private bool canMove;

    private Vector3 _currentInput;
    private int coef;

    public Transform Attack1Point;
    public float Attack1Range = 0.5f;
    public int Attack1Damage = 20;
    public float Attack1Rate = 2f;
    public float NextAttack1Time = 0f;

    public Transform Attack2Point;
    public float Attack2Range = 0.5f;
    public int Attack2Damage = 20;
    public float Attack2Rate = 2f;
    public float NextAttack2Time = 0f;
    
    public Transform Attack3Point;
    public float Attack3Range = 0.5f;
    public int Attack3Damage = 20;
    public float Attack3Rate = 2f;
    public float NextAttack3Time = 0f;

    public float DodgeRate = 2f;
    public float NextDodgeTime = 0f;

    private int currentHealth = 1600;

    public HealthBar healthbar;
    
    public LayerMask Player1Layer;
    public LayerMask Player2Layer;

    string[] animations = {"Attack2_1Animation", "Attack2_2Animation", "Attack2_3Animation", "Attack3_1Animation", "Attack3_2Animation", "Attack3_3Animation"};
    string[] animationsAir = {"Attack1_1Animation", "Attack1_2Animation", "Attack1_3Animation", "DodgeAnimation"};

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        Attack1Point = this.gameObject.transform.GetChild(0);
        Attack2Point = this.gameObject.transform.GetChild(1);
        Attack3Point = this.gameObject.transform.GetChild(2);

        _currentInput.x = 1;
        coef = 2;
        
        healthbar.SetMaxHealth(currentHealth);
    }
    
    private void Update()
    {
        _animator.SetBool("IsMoving", false);
        _animator.SetBool("Fall", false);

        
//foreach length
        for (int i = 0; i < 6; i++) {
            if (animations[i] == (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name).ToString()) {
                canMove = false;
                break;
            }
            else {
                canMove = true;
            }
        }

        for (int i = 0; i < 4; i++) {
            if (animationsAir[i] == (_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name).ToString() && _isGrounded) {
                canMove = false;
                break;
            }   
        }

        if ((_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name).ToString() == "Attack1_2Animation") {
            
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(Attack1Point.position, Attack1Range, Player2Layer);

            foreach(Collider2D player in hitPlayer) {
                player.GetComponent<Character2>().TakeDamage(Attack1Damage);
            } 

        }

        if ((_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name).ToString() == "Attack2_2Animation") {
            
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(Attack2Point.position, Attack2Range, Player2Layer);

            foreach(Collider2D player in hitPlayer) {
                player.GetComponent<Character2>().TakeDamage(Attack2Damage);
            }
        }

        if ((_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name).ToString() == "Attack3_2Animation") {

            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(Attack3Point.position, Attack3Range, Player2Layer);

            foreach(Collider2D player in hitPlayer) {
                player.GetComponent<Character2>().TakeDamage(Attack3Damage);
            }
        }

        if (canMove) {
            Move();
        }
        
        if (Input.GetKeyDown(KeyCode.W) && _isGrounded && canMove) {
            Jump();
        }

        if (Time.time >= NextAttack1Time) {
            if (Input.GetKeyDown(KeyCode.G) && canMove && !_isGrounded) {
            _animator.SetTrigger("Attack1");
            NextAttack1Time = Time.time + 1f / Attack1Rate;
        }
        }

        if (Time.time >= NextAttack2Time) {
            
            if (Input.GetKeyDown(KeyCode.H) && canMove && _isGrounded) {
            _animator.SetTrigger("Attack2");
            NextAttack2Time = Time.time + 1f / Attack2Rate;
        }
        }
        
        if (Time.time >= NextAttack3Time) {
            
            if (Input.GetKeyDown(KeyCode.J) && canMove && _isGrounded) {
            _animator.SetTrigger("Attack3");
            NextAttack3Time = Time.time + 1f / Attack3Rate;
        }
        }

        if (Time.time >= NextDodgeTime) {

            if (Input.GetKeyDown(KeyCode.Y) && canMove) {
            _animator.SetTrigger("Dodge");
            NextDodgeTime = Time.time + 1f / DodgeRate;
        }
        }
        
        if (!_isGrounded && _rigidbody.velocity.y < 0) {
            _animator.SetBool("Fall", true);
        }
        else{
            _animator.SetBool("Fall", false);
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        if(other.gameObject.tag == "Ground"){
            _isGrounded = true;
            _animator.SetBool("Grounded", true);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.tag == "Ground"){
            _isGrounded = false;
            _animator.SetBool("Grounded", false);
        }
    }

    private void Move() {
        _input = new Vector3(Input.GetAxis("Horizontal"), 0);
        transform.position += _input * _speed * Time.deltaTime;

        if (_input.x != 0 && _isGrounded) {
            _characterSprite.flipX = _input.x < 0; 

            if ((_currentInput.x > 0) != (_input.x > 0)) {
            _currentInput.x *= -1;
            coef *= -1;
            Attack1Point.position = Attack1Point.position + new Vector3(1.396f * coef, 0, 0);
            Attack2Point.position = Attack2Point.position + new Vector3(1.3f * coef, 0, 0);
            Attack3Point.position = Attack3Point.position + new Vector3(2.029f * coef, 0, 0);
            }

            _animator.SetBool("IsMoving", true);
            
        }
    }
    
    private void OnDrawGizmosSelected() {
        if (Attack1Point == null) {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Attack3Point.position, Attack3Range); 
    } 

    private void Jump() {
        _rigidbody.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
        _animator.SetTrigger("Jump");
    }

    public void TakeDamage(int damage) {
        
        if ((_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name).ToString() != "DodgeAnimation") {
            
            currentHealth -= damage;
            _animator.SetTrigger("Damage");

            healthbar.SetHealth(currentHealth);

            if (currentHealth < 0) {
            Die();
        }
        }
    }
    
    public void Die() {

        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<EdgeCollider2D>().enabled = false;
        DestroyImmediate(_rigidbody);

        _animator.SetBool("IsDead", true);

        this.enabled = false;
    
    }
}
