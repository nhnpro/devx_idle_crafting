public interface IBaseDataConverter
{
	void FillBaseData(JSONObject json, BaseData data);

	void FillJson(JSONObject json, BaseData data);
}
