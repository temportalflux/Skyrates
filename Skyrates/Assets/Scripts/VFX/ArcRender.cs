using UnityEngine;

namespace Skyrates.Effects
{

    // https://en.wikipedia.org/wiki/Projectile_motion
    // https://www.youtube.com/watch?v=iLlWirdxass
    // https://www.youtube.com/watch?v=TXHK1nPUOBE
    [RequireComponent(typeof(MeshFilter))]
    public class ArcRender : MonoBehaviour
    {

        public float Velocity;

        public float AngleDegrees;

        public float MeshWidthStart;

        public float MeshWidthEnd;

        [Range(0, 50)]
        public int Resolution;

        private int PositionCount
        {
            get { return this.Resolution + 1; }
        }

        private float ResolutionInv
        {
            get { return 1.0f / this.Resolution; }
        }

        private Mesh _mesh;
        private Vector3 _gravity;

        void Awake()
        {
            this._mesh = this.GetComponent<MeshFilter>().mesh;
            this._gravity = UnityEngine.Physics.gravity;
            this.RecalculateMesh();
        }

        void OnValidate()
        {
            if (this._mesh == null || !Application.isPlaying) return;
            this.RecalculateMesh();
        }

        private void RecalculateMesh()
        {
            this.RecalculateMesh(this.Velocity, Mathf.Deg2Rad * this.AngleDegrees);
        }

        /// <summary>
        /// Populates the positions in <see cref="LineRenderer"/> with the arc which represents the path that a projectile would take.
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="angleXRadians">The angle in radians around the local x axis</param>
        public void RecalculateMesh(float velocity, float angleXRadians)
        {
            float resolutionInv = this.ResolutionInv;
            Vector3[] arcPositions = this.CalculateArcPositions(velocity, angleXRadians, resolutionInv);
            
            this._mesh.Clear();
            // arc has 2 sides of the mesh
            Vector3[] verticies = new Vector3[this.PositionCount * 2];
            int[] triangles = new int[this.Resolution * 6 * 2]; // all quads are 2 triangles, which is 6 per segment (6 for both top and bottom of arc)

            // Iterate over all position data
            for (int iPosition = 0; iPosition < this.PositionCount; iPosition++)
            {
                float tLerp = (float) iPosition * resolutionInv;
                float meshWidth = (this.MeshWidthEnd * tLerp) + (this.MeshWidthStart * (1 - tLerp));
                // Set verticies
                // x in 3D is left/right, while z is distance (which is what x is in 2D)
                // Even verticies are on mesh right side
                verticies[iPosition * 2 + 0] = new Vector3(meshWidth * +0.5f, arcPositions[iPosition].y, arcPositions[iPosition].x);
                // Odd verticies are on mesh left side
                verticies[iPosition * 2 + 1] = new Vector3(meshWidth * -0.5f, arcPositions[iPosition].y, arcPositions[iPosition].x);

                // triangles not needed for last position
                if (iPosition == this.Resolution) continue;

                // Set triangles
                // clockwise
                triangles[iPosition * 12 + 0] = iPosition * 2;
                triangles[iPosition * 12 + 1] = triangles[iPosition * 12 + 4] = (iPosition * 2) + 1;
                triangles[iPosition * 12 + 2] = triangles[iPosition * 12 + 3] = (iPosition + 1) * 2;
                triangles[iPosition * 12 + 5] = (iPosition + 1) * 2 + 1;
                // counter clockwise
                triangles[iPosition * 12 + 6] = iPosition * 2;
                triangles[iPosition * 12 + 7] = triangles[iPosition * 12 + 10] = (iPosition + 1) * 2;
                triangles[iPosition * 12 + 8] = triangles[iPosition * 12 + 9] = (iPosition * 2) + 1;
                triangles[iPosition * 12 + 11] = (iPosition + 1) * 2 + 1;

            }

            this._mesh.vertices = verticies;
            this._mesh.triangles = triangles;

        }

        /// <summary>
        /// Calculate each position of the line render, based on the velocity and angle of trajectory. Positions are relative to Vector3.zero.
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="angleXRadians">The angle in radians around the local x axis</param>
        /// <param name="resolutionInv">The decimal percentage of the total number of positions that one position occupies (1 / resolution).</param>
        /// <returns></returns>
        private Vector3[] CalculateArcPositions(float velocity, float angleXRadians, float resolutionInv)
        {
            Vector3[] positions = new Vector3[this.PositionCount];

            if (velocity <= 0) return positions;

            float velocitySq = velocity * velocity;

            // Determine the maximum distance that a projectile would travel
            float maxDistance = (velocitySq * Mathf.Sin(2 * angleXRadians)) / this._gravity.y;

            float angleTan = Mathf.Tan(angleXRadians);
            float angleCosSq = Mathf.Cos(angleXRadians);
            angleCosSq *= angleCosSq;
            
            float angledSpeed = 2 * velocitySq * angleCosSq;

            // For each position in the arc
            for (int iPosition = 0; iPosition < this.PositionCount; iPosition++)
            {
                // Get how much of the arc is traversed
                float positionPercent = (float)iPosition * resolutionInv;
                // Get the distance from the start that this has traveled (in 2D, this is x)
                float x = positionPercent * maxDistance;
                // Get the height for the position in the arc (in 2D, this is y)
                float y = x * angleTan - ((this._gravity.y * x * x) / angledSpeed);
                // Store in arc positions
                positions[iPosition] = new Vector3(-x, -y);
            }

            return positions;
        }

    }

}
