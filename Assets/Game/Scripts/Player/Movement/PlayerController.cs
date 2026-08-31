using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Goru.Movement
{
    using Goru.Animation;
    using Goru.Audio;
    using Goru.Core;
    using Goru.Inputs;

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Configuration")]
        [SerializeField] private float moveSpeed = 2.0f;
        [SerializeField] private float walkingTired = 1.2f;
        [SerializeField] private float sprintSpeed = 5.335f;

        [Range(0.0f, 0.3f)][SerializeField] private float rotationSmoothTime = 0.12f;
        [SerializeField] private float speedChangeRate = 10.0f;

        [Header("Jump & Gravity")]
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -15.0f;
        [SerializeField] private float jumpTimeout = 0.50f;
        [SerializeField] private float fallTimeout = 0.15f;

        [Header("Ground Check")]
        [SerializeField] private float groundedOffset = 0.25f;
        [SerializeField] private float groundedRadius = 0.28f;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private LayerMask LayerFuego;

        private InputProvider _input;
        private PersonAnimationController _anim;
        private CharacterController _controller;
        private Camera _mainCamera;

        private float _speed;
        private float _animationBlend;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _targetRotation;

        [Header("Player Stats & Water")]
        public int vida = 20;
        public int energy = 200;
        [SerializeField] private float timeInvulnerable = 5f;
        [SerializeField] private Collider ColliderAgua;
        [SerializeField] private bool CubetaVacia = true;
        [SerializeField] private bool PuedeUsarAgua = false;
        [SerializeField] private bool estaEnAgua = false;
        [SerializeField] private bool CargandoAgua = false;
        [SerializeField] private bool EstaCercaDelFuego = false;
        private bool invulnerable = false;

        private UIDocument uiDocument;
        private VisualElement rootVisualElement;
        private VisualElement Derrota;

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<InputProvider>();
            _anim = GetComponent<PersonAnimationController>();
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            uiDocument = FindAnyObjectByType<UIDocument>();
            rootVisualElement = uiDocument.rootVisualElement;
            if (rootVisualElement != null)
            {
                Derrota = rootVisualElement.Q<VisualElement>("DerrotaVE");
            }
         
        }

        private void Start()
        {
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
            invulnerable = false;
            PuedeUsarAgua = false;
        }

        private void Update()
        {
            if (estaEnAgua && _input.BaldeRequested && CubetaVacia && !CargandoAgua)
            {
                StartCoroutine(CargarCubeta());
            }

            if (EstaCercaDelFuego && PuedeUsarAgua && _input.SecarRequested)
            {
                UsarAgua();
            }

            GroundedCheck();
            HandleJumpAndGravity();
            HandleMovement();
        }

        private void GroundedCheck()
        {
            Vector3 spherePos = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
            IsGrounded = Physics.CheckSphere(spherePos, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
            _anim?.SetGrounded(IsGrounded);
        }

        private void HandleMovement()
        {
            Vector2 moveInput = _input.MoveInput;
            float targetSpeed = GetTargetSpeed();

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = moveInput.magnitude;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            if (moveInput != Vector2.zero)
            {
                float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
                if (_mainCamera != null) targetRotation += _mainCamera.transform.eulerAngles.y;

                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _rotationVelocity, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, rotation, 0f);
                _targetRotation = targetRotation;
            }

            Vector3 targetDirection = Quaternion.Euler(0f, _targetRotation, 0f) * Vector3.forward;
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime);

            _anim?.UpdateMovement(_animationBlend, inputMagnitude, energy);
        }

        private float GetTargetSpeed()
        {
            Vector2 moveInput = _input.MoveInput;
            if (moveInput == Vector2.zero) return 0f;

            float targetSpeed = moveSpeed;
            if (_input.SprintRequested) targetSpeed = sprintSpeed;

            return targetSpeed;
        }

        private void HandleJumpAndGravity()
        {
            if (IsGrounded)
            {
                _fallTimeoutDelta = fallTimeout;
                _anim?.SetJump(false);
                _anim?.SetFreeFall(false);

                if (_verticalVelocity < 0f) _verticalVelocity = -2f;

                if (_input.JumpRequested && _jumpTimeoutDelta <= 0f)
                {
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    _anim?.SetJump(true);
                    _input.ConsumeJump();
                }

                if (_jumpTimeoutDelta >= 0f) _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = jumpTimeout;

                if (_fallTimeoutDelta >= 0f)
                    _fallTimeoutDelta -= Time.deltaTime;
                else
                    _anim?.SetFreeFall(true);
            }

            if (_verticalVelocity < _terminalVelocity)
                _verticalVelocity += gravity * Time.deltaTime;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Frutapala"))
            {
                energy += 30;
                Destroy(collision.gameObject);
            }
            else if (collision.gameObject.CompareTag("Murtilla"))
            {
                energy += 60;
                Destroy(collision.gameObject);
            }
            else if (collision.gameObject.CompareTag("Fire") && !invulnerable)
            {
                RecibirDano(1);
            }
        }

        public void OnTriggerStay(Collider collision)
        {
            if (collision.gameObject.CompareTag("Water"))
            {
                if (vida < 100) vida += 1;
                estaEnAgua = true;
            }
            else if (collision.gameObject.CompareTag("AguaProfunda"))
            {
                RecibirDano(25);
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.CompareTag("Fire")) EstaCercaDelFuego = true;
        }

        private void OnTriggerExit(Collider collision)
        {
            if (collision.CompareTag("Water")) estaEnAgua = false;
            if (collision.CompareTag("Fire")) EstaCercaDelFuego = false;
        }

        private void RecibirDano(int cantidad)
        {
            vida -= cantidad;
            if (vida <= 0)
            {
                Morir();
            }
            else
            {
                StartCoroutine(Invulnerabilidad());
            }
        }

        IEnumerator CargarCubeta()
        {
            CargandoAgua = true;
            Debug.Log("Cargando agua...");

            yield return new WaitForSeconds(3f);

            CubetaVacia = false;
            PuedeUsarAgua = true;
            CargandoAgua = false;
            Debug.Log("¡Balde lleno!");
        }

        void UsarAgua()
        {
            if (!PuedeUsarAgua || CubetaVacia) return;

            // Se consume el agua del balde
            PuedeUsarAgua = false;
            CubetaVacia = true; // <-- Habilita para poder recargar de nuevo en el agua

            if (ColliderAgua != null) ColliderAgua.enabled = true;

            StartCoroutine(DesactivarAgua());
        }

        IEnumerator DesactivarAgua()
        {
            yield return new WaitForSeconds(0.2f);

            // Detecta los fuegos cercanos en la capa LayerFuego
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 6f, LayerFuego);

            foreach (Collider hit in hitColliders)
            {
                // Busca el script Fuego en el objeto impactado o en sus componentes superiores
                Fuego fuegoScript = hit.GetComponentInParent<Fuego>();
                if (fuegoScript != null && !fuegoScript.extinguido)
                {
                    fuegoScript.TirarAgua();
                }
            }

            if (ColliderAgua != null) ColliderAgua.enabled = false;
        }

        IEnumerator Invulnerabilidad()
        {
            invulnerable = true;
            yield return new WaitForSeconds(timeInvulnerable);
            invulnerable = false;
        }

        void Morir()
        {
            vida = 0;
            _anim?.SetDeath(true);
            Debug.Log("se ejecuto la animacion");
            _speed = 0f;
            Derrota.style.display = DisplayStyle.Flex;
            Time.timeScale = 0f;
            
        }
    }
}