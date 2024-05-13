using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TempleRun {

    public class Gun : MonoBehaviour
    {
        [SerializeField]
        private Transform SpawnPosition;
        [SerializeField]
        private ParticleSystem ImpactParticleSystem;
        [SerializeField]
        private TrailRenderer BulletTrail;
        private int LayerObstacleNumber = 8;
        private int LayerMask;

        private void Start()
        {
            LayerMask = 1 << LayerObstacleNumber;
        }

        public void Shoot()
        {
            Vector3 positionMax = SpawnPosition.position + Vector3.up;
            Vector3 positionMin = SpawnPosition.position - Vector3.up;

            Debug.DrawRay(positionMax, SpawnPosition.forward*10f, Color.red, 10f);
            Debug.DrawRay(positionMin, SpawnPosition.forward*10f, Color.green, 10f);

            if (Physics.Raycast(positionMax, SpawnPosition.forward, out RaycastHit hitObsUp, Mathf.Infinity, LayerMask)) {
                TrailRenderer trail = Instantiate(BulletTrail, SpawnPosition.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hitObsUp));
            }
            else if (Physics.Raycast(positionMin, SpawnPosition.forward, out RaycastHit hitObsDown, Mathf.Infinity, LayerMask))
            {
                TrailRenderer trail = Instantiate(BulletTrail, SpawnPosition.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hitObsDown));
            }
            else {
                TrailRenderer trail = Instantiate(BulletTrail, SpawnPosition.position, Quaternion.identity);
                StartCoroutine(SpawnTrailInfinite(trail, SpawnPosition.position + SpawnPosition.forward*100f));
            }
        }

        private IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit Hit) {
            float time = 0;
            Vector3 startPosition = Trail.transform.position;

            while (time<1) {
                Trail.transform.position = Vector3.Lerp(startPosition, Hit.point, time);
                time += Time.deltaTime / Trail.time;

                yield return null;

            }
            Trail.transform.position = Hit.point;

            Instantiate(ImpactParticleSystem, Hit.point, Quaternion.LookRotation(Hit.normal));
            Destroy(Hit.collider.gameObject);

            Destroy(Trail.gameObject, Trail.time);
        }

        private IEnumerator SpawnTrailInfinite(TrailRenderer Trail, Vector3 EndPosition) {
            float time = 0;
            Vector3 startPosition = Trail.transform.position;

            while (time<1) {
                Trail.transform.position = Vector3.Lerp(startPosition, EndPosition, time);
                time += Time.deltaTime / Trail.time;

                yield return null;

            }
            Trail.transform.position = EndPosition;

            Destroy(Trail.gameObject, Trail.time);
        }
    }
}