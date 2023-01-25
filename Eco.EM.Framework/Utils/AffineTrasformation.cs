using Eco.Shared;
using Eco.Shared.Math;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Eco.EM.Framework.Utils
{

    /**
 * An affine transform.
 *
 * <p>This class is from the
 * <a href="http://geom-java.sourceforge.net/index.html">JavaGeom project</a>,
 * which is licensed under LGPL v2.1.</p>
 */
    public class AffineTransform
    {

        /**
         * coefficients for x coordinate.
         */
        private float m00, m01, m02, m03;

        /**
         * coefficients for y coordinate.
         */
        private float m10, m11, m12, m13;

        /**
         * coefficients for z coordinate.
         */
        private float m20, m21, m22, m23;

        // ===================================================================
        // constructors

        /**
         * Creates a new affine transform3D set to identity
         */
        public AffineTransform()
        {
            // init to identity matrix
            m00 = m11 = m22 = 1;
            m01 = m02 = m03 = 0;
            m10 = m12 = m13 = 0;
            m20 = m21 = m23 = 0;
        }

        public AffineTransform(float[] coefs)
        {
            if (coefs.Length == 9)
            {
                m00 = coefs[0];
                m01 = coefs[1];
                m02 = coefs[2];
                m10 = coefs[3];
                m11 = coefs[4];
                m12 = coefs[5];
                m20 = coefs[6];
                m21 = coefs[7];
                m22 = coefs[8];
            }
            else if (coefs.Length == 12)
            {
                m00 = coefs[0];
                m01 = coefs[1];
                m02 = coefs[2];
                m03 = coefs[3];
                m10 = coefs[4];
                m11 = coefs[5];
                m12 = coefs[6];
                m13 = coefs[7];
                m20 = coefs[8];
                m21 = coefs[9];
                m22 = coefs[10];
                m23 = coefs[11];
            }
            else
            {
                throw new ArgumentException("Input array must have 9 or 12 elements");
            }
        }

        public AffineTransform(float xx, float yx, float zx, float tx,
                               float xy, float yy, float zy, float ty, float xz, float yz,
                               float zz, float tz)
        {
            m00 = xx;
            m01 = yx;
            m02 = zx;
            m03 = tx;
            m10 = xy;
            m11 = yy;
            m12 = zy;
            m13 = ty;
            m20 = xz;
            m21 = yz;
            m22 = zz;
            m23 = tz;
        }

        // ===================================================================
        // accessors


        public bool IsIdentity()
        {
            if (m00 != 1)
                return false;
            if (m11 != 1)
                return false;
            if (m22 != 1)
                return false;
            if (m01 != 0)
                return false;
            if (m02 != 0)
                return false;
            if (m03 != 0)
                return false;
            if (m10 != 0)
                return false;
            if (m12 != 0)
                return false;
            if (m13 != 0)
                return false;
            if (m20 != 0)
                return false;
            if (m21 != 0)
                return false;
            if (m23 != 0)
                return false;
            return true;
        }

        /**
         * Returns the affine coefficients of the transform. Result is an array of
         * 12 float.
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float[] Coefficients()
        {
            return new float[] { m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23 };
        }

        /**
         * Computes the determinant of this transform. Can be zero.
         *
         * @return the determinant of the transform.
         */
        private float Determinant()
        {
            return m00 * (m11 * m22 - m12 * m21) - m01 * (m10 * m22 - m20 * m12)
                    + m02 * (m10 * m21 - m20 * m11);
        }

        /**
         * Computes the inverse affine transform.
         */

        public AffineTransform Inverse()
        {
            float det = this.Determinant();
            return new AffineTransform(
                    (m11 * m22 - m21 * m12) / det,
                    (m21 * m01 - m01 * m22) / det,
                    (m01 * m12 - m11 * m02) / det,
                    (m01 * (m22 * m13 - m12 * m23) + m02 * (m11 * m23 - m21 * m13)
                            - m03 * (m11 * m22 - m21 * m12)) / det,
                    (m20 * m12 - m10 * m22) / det,
                    (m00 * m22 - m20 * m02) / det,
                    (m10 * m02 - m00 * m12) / det,
                    (m00 * (m12 * m23 - m22 * m13) - m02 * (m10 * m23 - m20 * m13)
                            + m03 * (m10 * m22 - m20 * m12)) / det,
                    (m10 * m21 - m20 * m11) / det,
                    (m20 * m01 - m00 * m21) / det,
                    (m00 * m11 - m10 * m01) / det,
                    (m00 * (m21 * m13 - m11 * m23) + m01 * (m10 * m23 - m20 * m13)
                            - m03 * (m10 * m21 - m20 * m11)) / det);
        }

        // ===================================================================
        // general methods

        /**
         * Returns the affine transform created by applying first the affine
         * transform given by {@code that}, then this affine transform.
         *
         * @param that the transform to apply first
         * @return the composition this * that
         */
        public AffineTransform Concatenate(AffineTransform that)
        {
            float n00 = m00 * that.m00 + m01 * that.m10 + m02 * that.m20;
            float n01 = m00 * that.m01 + m01 * that.m11 + m02 * that.m21;
            float n02 = m00 * that.m02 + m01 * that.m12 + m02 * that.m22;
            float n03 = m00 * that.m03 + m01 * that.m13 + m02 * that.m23 + m03;
            float n10 = m10 * that.m00 + m11 * that.m10 + m12 * that.m20;
            float n11 = m10 * that.m01 + m11 * that.m11 + m12 * that.m21;
            float n12 = m10 * that.m02 + m11 * that.m12 + m12 * that.m22;
            float n13 = m10 * that.m03 + m11 * that.m13 + m12 * that.m23 + m13;
            float n20 = m20 * that.m00 + m21 * that.m10 + m22 * that.m20;
            float n21 = m20 * that.m01 + m21 * that.m11 + m22 * that.m21;
            float n22 = m20 * that.m02 + m21 * that.m12 + m22 * that.m22;
            float n23 = m20 * that.m03 + m21 * that.m13 + m22 * that.m23 + m23;
            return new AffineTransform(
                    n00, n01, n02, n03,
                    n10, n11, n12, n13,
                    n20, n21, n22, n23);
        }

        /**
         * Return the affine transform created by applying first this affine
         * transform, then the affine transform given by {@code that}.
         *
         * @param that the transform to apply in a second step
         * @return the composition that * this
         */
        public AffineTransform PreConcatenate(AffineTransform that)
        {
            float n00 = that.m00 * m00 + that.m01 * m10 + that.m02 * m20;
            float n01 = that.m00 * m01 + that.m01 * m11 + that.m02 * m21;
            float n02 = that.m00 * m02 + that.m01 * m12 + that.m02 * m22;
            float n03 = that.m00 * m03 + that.m01 * m13 + that.m02 * m23 + that.m03;
            float n10 = that.m10 * m00 + that.m11 * m10 + that.m12 * m20;
            float n11 = that.m10 * m01 + that.m11 * m11 + that.m12 * m21;
            float n12 = that.m10 * m02 + that.m11 * m12 + that.m12 * m22;
            float n13 = that.m10 * m03 + that.m11 * m13 + that.m12 * m23 + that.m13;
            float n20 = that.m20 * m00 + that.m21 * m10 + that.m22 * m20;
            float n21 = that.m20 * m01 + that.m21 * m11 + that.m22 * m21;
            float n22 = that.m20 * m02 + that.m21 * m12 + that.m22 * m22;
            float n23 = that.m20 * m03 + that.m21 * m13 + that.m22 * m23 + that.m23;
            return new AffineTransform(
                    n00, n01, n02, n03,
                    n10, n11, n12, n13,
                    n20, n21, n22, n23);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AffineTransform Translate(Vector3i vec)
        {
            return Translate(vec.x, vec.y, vec.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AffineTransform Translate(float x, float y, float z)
        {
            return Concatenate(new AffineTransform(1, 0, 0, x, 0, 1, 0, y, 0, 0, 1, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AffineTransform RotateXDeg(float pAngleDeg)
        {
            return RotateX(Mathf.DegToRad(pAngleDeg));
        }

        public AffineTransform RotateX(float theta)
        {
            float cot = MathF.Cos(theta);
            float sit = MathF.Sin(theta);
            return Concatenate(
                    new AffineTransform(
                            1, 0, 0, 0,
                            0, cot, -sit, 0,
                            0, sit, cot, 0));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AffineTransform RotateYDeg(float pAngleDeg)
        {
            return RotateY(Mathf.DegToRad(pAngleDeg));
        }

        public AffineTransform RotateY(float theta)
        {
            float cot = MathF.Cos(theta);
            float sit = MathF.Sin(theta);
            return Concatenate(
                    new AffineTransform(
                            cot, 0, sit, 0,
                            0, 1, 0, 0,
                            -sit, 0, cot, 0));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AffineTransform RotateZDeg(float pAngleDeg)
        {
            return RotateZ(Mathf.DegToRad(pAngleDeg));
        }

        public AffineTransform RotateZ(float theta)
        {
            float cot = MathF.Cos(theta);
            float sit = MathF.Sin(theta);
            return Concatenate(
                    new AffineTransform(
                            cot, -sit, 0, 0,
                            sit, cot, 0, 0,
                            0, 0, 1, 0));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AffineTransform Scale(float s)
        {
            return Scale(s, s, s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AffineTransform Scale(float sx, float sy, float sz)
        {
            return Concatenate(new AffineTransform(sx, 0, 0, 0, 0, sy, 0, 0, 0, 0, sz, 0));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AffineTransform Scale(Vector3i vec)
        {
            return Scale(vec.x, vec.y, vec.z);
        }

        public Vector3i Apply(Vector3i vector)
        {
            return new Vector3i(
                   (int)Math.Round(vector.x * m00 + vector.y * m01 + vector.Z * m02 + m03),
                      (int)Math.Round(vector.x * m10 + vector.y * m11 + vector.Z * m12 + m13),
                     (int)Math.Round(vector.x * m20 + vector.y * m21 + vector.Z * m22 + m23));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AffineTransform Combine(AffineTransform other)
        {
            return Concatenate(other);
        }

        public override string ToString()
        {
            return String.Format("Affine[%g %g %g %g, %g %g %g %g, %g %g %g %g]}", m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23);
        }
    }
}
