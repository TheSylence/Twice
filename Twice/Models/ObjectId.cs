namespace Twice.Models
{
	internal enum ObjectType
	{
		User,
		Tweet,
		Message
	}

	internal class ObjectId
	{
		public ObjectId( ulong id, ObjectType type )
		{
			Id = id;
			Type = type;
		}

		public override bool Equals( object obj )
		{
			ObjectId other = obj as ObjectId;
			if( other == null )
			{
				return false;
			}

			return other.Id == Id && other.Type == Type;
		}

		public override int GetHashCode()
		{
			int hash = 23;

			unchecked
			{
				hash = hash * 17 + Id.GetHashCode();
				hash = hash * 17 + Type.GetHashCode();
			}

			return hash;
		}

		public ulong Id { get; }
		public ObjectType Type { get; }
	}
}