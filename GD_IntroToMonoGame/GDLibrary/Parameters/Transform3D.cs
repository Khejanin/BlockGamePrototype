using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    /// <summary>
    /// Holds data related to actor (e.g. player, pickup, decorator, architecture, camera) position
    /// </summary>
    public class Transform3D : ICloneable
    {
        private Vector3 translation, localTranslation, rotationInDegrees, localRotationInDegrees, scale, localScale;
        private Vector3 look, up; //right = look x up
        private Vector3 originalLook;
        private Vector3 originalUp;
        private Quaternion rotation;

        public Transform3D parent;
        private List<Transform3D> children;

        //add a clean/dirty flag later
        public Matrix World
        {
            get
            {
                return Matrix.Identity
                    * Matrix.CreateScale(this.scale)
                    //* Matrix.CreateRotationY(MathHelper.ToRadians(this.rotationInDegrees.Y))
                    //    * Matrix.CreateRotationX(MathHelper.ToRadians(this.rotationInDegrees.X))
                    //        * Matrix.CreateRotationZ(MathHelper.ToRadians(this.rotationInDegrees.Z))
                    //* Matrix.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(this.rotationInDegrees.X))
                    //* Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(this.rotationInDegrees.Y))
                    //* Matrix.CreateFromAxisAngle(Vector3.Forward, MathHelper.ToRadians(this.rotationInDegrees.Z))
                    * Matrix.CreateFromQuaternion(Rotation)
                    * Matrix.CreateTranslation(this.translation)
                    * (parent != null
                        ? parent.World
                        : Matrix.Identity);
            }
        }

        public Vector3 Look
        {
            get
            {
                look.Normalize(); //less-cpu intensive than Vector3.Normalize()
                return look;
            }
            set
            {
                this.look = value;
            }
        }
        public Vector3 Up
        {
            get
            {
                up.Normalize(); //less-cpu intensive than Vector3.Normalize()
                return up;
            }
            set
            {
                this.up = value;
            }
        }
        public Vector3 Right
        {
            get
            {
                return Vector3.Normalize(Vector3.Cross(this.look, this.up));
            }
        }

        public Vector3 Translation {
            get
            {
                return this.translation;
            }
            set
            {
                this.translation = value;
                UpdateChildren(value, Vector3.Zero, Vector3.Zero);
            } 
        }

        public Vector3 LocalTranslation
        {
            get
            {
                return this.localTranslation;
            }
            set
            {
                Translation = (parent != null) ? parent.Translation + value : value;
                this.localTranslation = value;
            }
        }

        private Vector3 originalRotationInDegrees;

        public Quaternion Rotation
        {
            get
            {
                return this.rotation;
            }
            set
            {
                this.rotation = value;
                this.rotation.Normalize();
            }
        }

        //public Vector3 RotationInDegrees
        //{
        //    get
        //    {
        //        return this.rotationInDegrees;
        //    }
        //    set
        //    {
        //        this.rotationInDegrees = value;
        //        UpdateChildren(Vector3.Zero, value, Vector3.Zero);
        //    }
        //}

        //public Vector3 LocalRotationInDegrees
        //{
        //    get
        //    {
        //        return this.localRotationInDegrees;
        //    }
        //    set
        //    {
        //        RotationInDegrees = (parent != null) ? RotationInDegrees + value : value;
        //        this.localRotationInDegrees = value;
        //    }
        //}

        public Vector3 Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                this.scale = value;
                UpdateChildren(Vector3.Zero, Vector3.Zero, value);
            }
        }

        public Vector3 LocalScale
        {
            get
            {
                return this.localScale;
            }
            set
            {
                Scale = (parent != null) ? parent.Scale + value : value;
                this.localScale = value;
            }
        }

        //constructor suitable for Camera3D (i.e. no rotation or scale)
        public Transform3D(Vector3 translation, Vector3 look, Vector3 up) : this(translation, Vector3.Zero, Vector3.One,
               look, up)
        {

        }

        //constructor suitable for drawn actors
        public Transform3D(Vector3 translation, Vector3 rotationInDegrees, 
            Vector3 scale, Vector3 look, Vector3 up)
        {
            this.children = new List<Transform3D>();
            this.Translation = translation;
            //this.originalRotationInDegrees = this.RotationInDegrees = rotationInDegrees;
            this.Scale = scale;
            this.originalLook = this.Look = look;
            this.originalUp = this.Up = up;
            //rotMatrix = Matrix.Identity * Matrix.CreateFromYawPitchRoll(rotationInDegrees.Y, rotationInDegrees.X, rotationInDegrees.Z);
            Rotation = Quaternion.Identity * Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(rotationInDegrees.Y), MathHelper.ToRadians(rotationInDegrees.X), MathHelper.ToRadians(rotationInDegrees.Z));
        }

        public void TranslateBy(Vector3 delta)
        {
            Translation += delta;
        }

        public void RotateAroundUpBy(float magnitude)
        {
            //to do...
        }

        public void RotateBy(Vector3 axisAndMagnitude, bool updateLookAndUp = true)
        {
            //add this statement to allow us to add/subtract from whatever the current rotation is
            Vector3 rotation = this.originalRotationInDegrees + axisAndMagnitude;

            //explain: yaw, pitch, roll
            //create a new "XYZ" axis to rotate around using the (x,y,0) values from mouse and any current rotation
            Matrix rotMatrix = Matrix.CreateFromYawPitchRoll(
                MathHelper.ToRadians(rotation.Y), //Pitch
                MathHelper.ToRadians(rotation.X), //Yaw
                MathHelper.ToRadians(rotation.Z)); //Roll

            if (updateLookAndUp)
            {
                //update the look and up vector (i.e. rotate them both around this new "XYZ" axis)
                this.look = Vector3.Transform(this.originalLook, rotMatrix);
                this.up = Vector3.Transform(this.originalUp, rotMatrix);
            }

            //UpdateChildren(Vector3.Zero, axisAndMagnitude, Vector3.Zero);
        }

        public void Rotate(Vector3 axisAndMagnitude)
        {
            Quaternion rot = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(axisAndMagnitude.X))
                            * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(axisAndMagnitude.Y))
                            * Quaternion.CreateFromAxisAngle(Vector3.Forward, MathHelper.ToRadians(axisAndMagnitude.Z));

            Rotation *= Quaternion.Inverse(Rotation) * rot * Rotation;
        }

        public void Rotate(Quaternion rot)
        {
            Rotation *= Quaternion.Inverse(Rotation) * rot * Rotation;
        }

        public void SetParent(ref Transform3D parent)
        {
            this.parent = parent;
            parent.children.Add(this);
        }

        private void UpdateChildren(Vector3 addedPos, Vector3 addedRotation, Vector3 addedScale)
        {
            //foreach (Transform3D child in children)
            //{
            //    child.RotateBy(addedRotation);
            //    child.TranslateBy(addedPos);
            //    child.Scale += addedScale;
            //}
        }

        public object Clone()
        {
            return new Transform3D(this.translation, this.rotationInDegrees, this.scale,
                this.look, this.up);
        }
        
    }
}
