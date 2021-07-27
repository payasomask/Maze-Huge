using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorAttributeAnimator : MonoBehaviour {

	[System.Serializable]
	public class ColorAttribute{
		public int frame_no;
		public Color color;
		public EasingAttribute EaingAttribute = EasingAttribute.SineEaseInOut;
	}
	public enum EasingAttribute
	{
		SineEaseInOut,
		easeInOutBack,
		easeOutElastic,
		Linear
	}
	// public enum Attribute{
	// 	SCALE,
	// 	COLOR
	// }
	// public Attribute controlled_attr =Attribute.SCALE;
	public List<ColorAttribute> key_list =new List<ColorAttribute>();

	public int total_frames =0;

	public int start_at =0;
	public float fps =30f;
	public bool loop =false;

	int curr_no =0;
	float fps_counter =0f;
	SpriteRenderer sr =null;

	bool done =false;

	// Use this for initialization
	float frame_duration =0f;
	void Start () {
		sr =GetComponent<SpriteRenderer>();
		curr_no =start_at;
		frame_duration =1f/fps;

		if (key_list.Count==1 && loop){
			sr.color =key_list[0].color;
			done =true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (done)
			return;

		fps_counter+=Time.deltaTime;
		if (fps_counter>frame_duration*total_frames){
			fps_counter =fps_counter%(total_frames*frame_duration);
		}

		if (fps_counter<= key_list[0].frame_no*frame_duration){
			//interpolate from the last key to the first key
			float last_seg =(total_frames-key_list[key_list.Count-1].frame_no)*frame_duration;
			float first_seg =key_list[0].frame_no*frame_duration;

			float time_elapsed =last_seg+fps_counter;
			intepolate_color(time_elapsed, key_list[key_list.Count-1].color, key_list[0].color-key_list[key_list.Count-1].color, last_seg+first_seg, key_list[0].EaingAttribute);

		}else
		if (fps_counter>= key_list[key_list.Count-1].frame_no*frame_duration){
			//interpolate from the last key to the first key
			float last_seg =(total_frames-key_list[key_list.Count-1].frame_no)*frame_duration;
			float first_seg =key_list[0].frame_no*frame_duration;

			float time_elapsed =fps_counter-(key_list[key_list.Count-1].frame_no*frame_duration);
			intepolate_color(time_elapsed, key_list[key_list.Count-1].color, key_list[0].color-key_list[key_list.Count-1].color, last_seg+first_seg, key_list[0].EaingAttribute);

		}else{
			//interpolate between keys
			for (int i=0;i<key_list.Count;++i){
				if (key_list[i].frame_no*frame_duration>=fps_counter){
					float seg =(key_list[i].frame_no-key_list[i-1].frame_no)*frame_duration;
					float time_elapsed =fps_counter-(key_list[i-1].frame_no*frame_duration);
					intepolate_color(time_elapsed, key_list[i-1].color, key_list[i].color-key_list[i-1].color, seg, key_list[i].EaingAttribute);

					break;
				}
			}

		}
		
	}

	void intepolate_color(float time_elapsed, Color start, Color diff, float total_time, EasingAttribute targetEasing){
		//預設使用SineEaseInOut
		float color_red =(float)CurveUtil.SineEaseInOut(time_elapsed, start.r, diff.r, total_time);
		float color_green =(float)CurveUtil.SineEaseInOut(time_elapsed, start.g, diff.g, total_time);
		float color_blue =(float)CurveUtil.SineEaseInOut(time_elapsed, start.b, diff.b, total_time);
		float color_alpha =(float)CurveUtil.SineEaseInOut(time_elapsed, start.a, diff.a, total_time);

		if (targetEasing == EasingAttribute.easeInOutBack)
		{
			color_red = (float)CurveUtil.BackEaseInOut(time_elapsed, start.r, diff.r, total_time);
			color_green = (float)CurveUtil.BackEaseInOut(time_elapsed, start.g, diff.g, total_time);
			color_blue = (float)CurveUtil.BackEaseInOut(time_elapsed, start.b, diff.b, total_time);
			color_alpha = (float)CurveUtil.BackEaseInOut(time_elapsed, start.a, diff.a, total_time);
		}
		else if (targetEasing == EasingAttribute.easeOutElastic)
		{
			color_red = (float)CurveUtil.ElasticEaseOut(time_elapsed, start.r, diff.r, total_time);
			color_green = (float)CurveUtil.ElasticEaseOut(time_elapsed, start.g, diff.g, total_time);
			color_blue = (float)CurveUtil.ElasticEaseOut(time_elapsed, start.b, diff.b, total_time);
			color_alpha = (float)CurveUtil.ElasticEaseOut(time_elapsed, start.a, diff.a, total_time);
		}
		else if (targetEasing == EasingAttribute.Linear)
		{
			color_red = (float)CurveUtil.Linear(time_elapsed, start.r, diff.r, total_time);
			color_green = (float)CurveUtil.Linear(time_elapsed, start.g, diff.g, total_time);
			color_blue = (float)CurveUtil.Linear(time_elapsed, start.b, diff.b, total_time);
			color_alpha = (float)CurveUtil.Linear(time_elapsed, start.a, diff.a, total_time);
		}

		sr.color =new Color(color_red, color_green, color_blue, color_alpha);
	}

	public void FromJsonOverwrite(string serialized_json_string){
		Dictionary<string, object> properties =(Dictionary<string, object>)MiniJSON.Json.Deserialize(serialized_json_string);
		total_frames =(int)(long)properties["total_frames"];
		start_at =(int)(long)properties["start_at"];
		fps = UtilityHelper.toFloat(properties["fps"]);
		loop =(bool)properties["loop"];

		key_list.Clear();
		List<object> key_list_array =(List<object>)properties["key_list"];
		for(int i=0;i<key_list_array.Count;++i){
			Dictionary<string, object> key_list_entity =(Dictionary<string, object>)key_list_array[i];
			ColorAttribute va =new ColorAttribute();
			key_list.Add(va);

			va.frame_no =(int)(long)key_list_entity["frame_no"];
			
			Dictionary<string, object> vec_dic =(Dictionary<string, object>)key_list_entity["color"];
			va.color =new Color(UtilityHelper.toFloat(vec_dic["r"]), UtilityHelper.toFloat(vec_dic["g"]), UtilityHelper.toFloat(vec_dic["b"]), UtilityHelper.toFloat(vec_dic["a"]));
		}
	}
}
