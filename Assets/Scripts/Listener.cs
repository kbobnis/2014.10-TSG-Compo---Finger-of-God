
public interface Listener<T, Y>
{
	void Clear(T t);
	void Inform(T t, Y delta);
}

