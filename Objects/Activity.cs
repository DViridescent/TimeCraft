using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects;

public class Activity : IActivity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Path { get; private set; }

    // 修改构造函数，增加路径的构建逻辑
    public Activity(string name, Activity? parent = null, string description = "")
    {
        Name = name;
        Description = description;
        // 如果有父活动，则路径为父活动的路径加上当前活动的名称，否则路径就是当前活动的名称
        Path = parent == null ? Name : $"{parent.Path}/{Name}";
    }

    // 第一层：生活的最基本状态
    /// <summary>
    /// 活着的状态，是所有活动的根。
    /// </summary>
    public readonly static Activity Alive = new("活着");

    // 第二层：基本的日常状态分类
    /// <summary>
    /// 工作状态，包括所有上班期间的活动。
    /// </summary>
    public readonly static Activity Working = new("上班", Alive);
    /// <summary>
    /// 非工作状态，包括所有下班后的活动。
    /// </summary>
    public readonly static Activity OffWork = new("下班", Alive);

    // 第三层：上班状态的进一步细分
    /// <summary>
    /// 上班期间进行的工作活动。
    /// </summary>
    public readonly static Activity WorkingJob = new("工作", Working);
    /// <summary>
    /// 上班期间非工作的活动，例如上网浏览、聊天等。
    /// </summary>
    public readonly static Activity Slacking = new("摸鱼", Working);
    /// <summary>
    /// 工作时间内的用餐活动。
    /// </summary>
    public readonly static Activity Eating = new("吃饭", Working);
    /// <summary>
    /// 生理需求活动。
    /// </summary>
    public readonly static Activity Toilet = new("上厕所", Working);

    // 第四层及以下：具体的工作活动
    /// <summary>
    /// 编写代码的工作活动。
    /// </summary>
    public readonly static Activity Coding = new("编码", WorkingJob);
    /// <summary>
    /// 审查他人代码的活动。
    /// </summary>
    public readonly static Activity CodeReview = new("代码审查", WorkingJob);
    /// <summary>
    /// 参与工作相关会议的活动。
    /// </summary>
    public readonly static Activity Meeting = new("会议", WorkingJob);
    /// <summary>
    /// 阅读或编写需求文档等文档工作。
    /// </summary>
    public readonly static Activity Documentation = new("文档工作", WorkingJob);
    /// <summary>
    /// 帮助同事解决技术问题的活动。
    /// </summary>
    public readonly static Activity Assistance = new("协助", WorkingJob);

    // 上班摸鱼的具体活动可以在这里添加

}