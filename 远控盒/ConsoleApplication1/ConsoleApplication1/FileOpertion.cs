using System;
using System.Configuration; 

namespace ConsoleApplication1
{
  public  class FileOpertion
    {
      /// <summary>
      /// 用户密码
      /// </summary>
      public string UserPwd { get; set; }

      /// <summary>
      /// 用户设置的额定电压
      /// </summary>
      public string UserSetRatedVoltage { get; set; }

      /// <summary>
      /// 用户设置的额定电流
      /// </summary>
      public string UserSetRatedCurrent { get; set; }

      /// <summary>
      /// 获取用户密码
      /// </summary>
      /// <returns></returns>
      public string getUserPwd()
      {
          return ConfigurationManager.AppSettings["UserPwd"];
      }

      /// <summary>
      /// 获取用户设置的额定电压
      /// </summary>
      /// <returns></returns>
      public string getUserSetRatedVoltage()
      {
          return ConfigurationManager.AppSettings["UserSetRatedVoltage"];
      }

      /// <summary>
      /// 获取用户设置的额定电流
      /// </summary>
      /// <returns></returns>
      public string getUserSetRatedCurrent()
      {
          return ConfigurationManager.AppSettings["UserSetRatedCurrent"];
      }


      /// <summary>
      /// 修改用户密码
      /// </summary> 
      public void setUserPwd(string newKeyValue)
      {
          modifyItem("UserPwd", newKeyValue);
      }

      /// <summary>
      /// 修改额定电压
      /// </summary> 
      public void setRatedVoltage(string newKeyValue)
      {
          modifyItem("UserSetRatedVoltage", newKeyValue);
      }

      /// <summary>
      /// 修改额定电流
      /// </summary> 
      public void setSetRatedCurrent(string newKeyValue)
      {
          modifyItem("UserSetRatedCurrent", newKeyValue);
      }

      /// <summary>
      /// 修改App文件值
      /// </summary>
      /// <param name="keyName">键</param>
      /// <param name="newKeyValue">值</param>
      void modifyItem(string keyName, string newKeyValue)
      {
          //修改配置文件中键为keyName的项的值
          Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
          config.AppSettings.Settings[keyName].Value = newKeyValue;
          config.Save(ConfigurationSaveMode.Modified);
          ConfigurationManager.RefreshSection("appSettings");
      }
    }
}
