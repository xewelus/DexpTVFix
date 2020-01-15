using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Threading;
using Common;
using Common.Classes;

namespace DexpBugDetectorWpf
{
	public static class Capturer
	{
		private static Session session;
		public static void Start(Action<string> onComplete = null, Action<Exception> onError = null, int threadsCount = 2, TimeSpan? duraction = null)
		{
			if (session != null)
			{
				throw new Exception("Предыдущая запись еще не завершена.");
			}

			string captureFolder = GetCaptureFolder();

			string subfolder = string.Format("{0:yyyy-MM-dd HH-mm-ss ffffff}", DateTime.Now);
			string folder = FS.Combine(captureFolder, subfolder);
			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}

			if (duraction == null)
			{
				duraction = TimeSpan.FromSeconds(30);
			}

			DateTime endDate = DateTime.Now.Add(duraction.Value);
			session = new Session(folder, threadsCount, endDate, onComplete, onError);
		}

		public static string GetCaptureFolder()
		{
			string folder = ConfigurationManager.AppSettings["CaptureFolder"];
			if (string.IsNullOrEmpty(folder))
			{
				folder = FS.GetAppFolder("capture");
			}
			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}
			return folder;
		}

		public static void Stop()
		{
			if (session != null)
			{
				session.StopSession();
				session = null;
			}
		}

		private class Session
		{
			private readonly string folder;
			private readonly Action<string> onComplete;
			private readonly Action<Exception> onError;
			private readonly DateTime endDate;
			private readonly PairsList<Bitmap, DateTime> toSave = new PairsList<Bitmap, DateTime>();
			private bool isCompleting;

			public Session(string folder, int threadsCount, DateTime endDate, Action<string> onComplete, Action<Exception> onError)
			{
				this.folder = folder;
				this.endDate = endDate;
				this.onComplete = onComplete;
				this.onError = onError;

				Thread saveThread = new Thread(this.SaveThreadMethod);
				saveThread.Start();

				for (int i = 0; i < threadsCount; i++)
				{
					Thread captureThread = new Thread(this.CaptureThreadMethod);
					captureThread.Start();
				}
			}

			private void CaptureThreadMethod()
			{
				try
				{
					while (!this.isCompleting)
					{
						if (DateTime.Now >= this.endDate)
						{
							this.StopSession();
							return;
						}

						GDI.CaptureScreen(this.AddToList, false);
					}
				}
				catch (Exception ex)
				{
					this.ShowError(ex);
				}
			}

			private void ShowError(Exception ex)
			{
				if (this.onError != null)
				{
					this.onError.Invoke(ex);
				}
			}

			private void SaveThreadMethod()
			{
				try
				{
					while (!this.isCompleting || this.toSave.Count > 0)
					{
						KeyValuePair<Bitmap, DateTime>? pair = null;
						lock (this.toSave)
						{
							if (this.toSave.Count > 0)
							{
								pair = this.toSave[0];
								this.toSave.RemoveAt(0);
							}
						}

						if (pair == null)
						{
							Thread.Sleep(10);
							continue;
						}

						Bitmap bitmap = pair.Value.Key;
						string file = FS.Combine(this.folder, string.Format(@"{0:yyyyMMdd HH mm ss ffffff}.png", pair.Value.Value));
						bitmap.Save(file);
						bitmap.Dispose();
					}
				}
				catch (Exception ex)
				{
					this.ShowError(ex);
				}
				finally
				{
					if (this.onComplete != null)
					{
						this.onComplete.Invoke(this.folder);
					}
				}
			}

			private void AddToList(Bitmap bitmap)
			{
				lock (this.toSave)
				{
					this.toSave.Add(bitmap, DateTime.Now);
				}
			}

			public void StopSession()
			{
				this.isCompleting = true;
			}
		}
	}
}
