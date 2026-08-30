using UnityEngine;

namespace Goru.Animation
{
    using Goru.Audio;
    using Goru.Core;

    [RequireComponent(typeof(Animator))]
    public class PersonAnimationController : MonoBehaviour, IPlayerAnimator
    {
        private Animator _anim;
        private PlayerAudioController _audioController;

        private int _idSpeed;
        private int _idGrounded;
        private int _idJump;
        private int _idFreeFall;
        private int _idMotionSpeed;
        private int _idResistence;
        private int _idIsAlert;
        private int _idEat;
        private int _idDeath;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _audioController = GetComponent<PlayerAudioController>();
            AssignIDs();
        }

        private void AssignIDs()
        {
            _idSpeed = Animator.StringToHash("Speed");
            _idGrounded = Animator.StringToHash("Grounded");
            _idJump = Animator.StringToHash("Jump");
            _idFreeFall = Animator.StringToHash("FreeFall");
            _idMotionSpeed = Animator.StringToHash("MotionSpeed");
            _idResistence = Animator.StringToHash("Resistence");
            _idIsAlert = Animator.StringToHash("IsAlert");
            _idEat = Animator.StringToHash("Eat");

        }

        public void UpdateMovement(float speedBlend, float motionSpeed, float currentResistence)
        {
            _anim.SetFloat(_idSpeed, speedBlend);
            _anim.SetFloat(_idResistence, currentResistence);
            _anim.SetFloat(_idMotionSpeed, motionSpeed);
            
        }

        public void SetJump(bool value) => _anim.SetBool(_idJump, value);
        public void SetFreeFall(bool value) => _anim.SetBool(_idFreeFall, value);
        public void SetGrounded(bool value) => _anim.SetBool(_idGrounded, value);
        public void SetEat(bool value) => _anim.SetBool(_idEat, value);
        public void SetAlert(bool value) => _anim.SetBool(_idIsAlert, value);
        public void SetDeath (bool value) => _anim.SetBool(_idDeath, value);

        private void OnFootstep(AnimationEvent evt)
        {
            if (_audioController != null)
                _audioController.OnFootstep(evt);
            
        }

        private void OnLand(AnimationEvent evt)
        {
            if (_audioController != null)
            _audioController.OnLand(evt);
        }
    }
}

