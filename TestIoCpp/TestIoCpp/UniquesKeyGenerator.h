
#ifndef UNIQUE_KEY_GENERATOR
#define UNIQUE_KEY_GENERATOR
namespace Psycho
{
	
	class UniqueKeyGenerator
	{
	public:
		template<class Key>
		static int Get()
		{
			static int nextKey = m_NextUniquekey++;
			return nextKey;
		}
	private:
		static int m_NextUniquekey;
	};

}
#endif