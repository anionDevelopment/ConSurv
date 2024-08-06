using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuousSurveillanceBackend.Core.Model.MoveDirections
{
    public abstract class MoveDirection
    {
        public abstract void Accept(IMoveDirectionVisitor visitor);
        public abstract T Accept<T>(IMoveDirectionVisitor<T> visitor);
    }
    public interface IMoveDirectionVisitor
    {
        void Handle(MoveDown moveDown);
        void Handle(MoveLeft moveLeft);
        void Handle(MoveRight moveRight);
        void Handle(MoveUp moveUp);
    }
    public interface IMoveDirectionVisitor<T>
    {
        T Handle(MoveDown moveDown);
        T Handle(MoveLeft moveLeft);
        T Handle(MoveRight moveRight);
        T Handle(MoveUp moveUp);
    }
}
