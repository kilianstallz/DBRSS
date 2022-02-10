namespace DBRSS.Buffer; 

public class ThresholdService
{
  private int Counter { get; set; }
  private int MaxCount { get; set; }
  private System.Timers.Timer Timer { get; set; }
    
  public event EventHandler OnTrigger;

  public ThresholdService()
  {
    this.MaxCount = 5;
    this.Counter = 0;
    this.Timer = new System.Timers.Timer();
    this.Timer.Interval = 5000;
    this.Timer.Enabled = true;
    this.Timer.Elapsed += HandleTimer;
  }

  private void HandleTrigger()
  {
    this.Timer.Enabled = false;
    this.OnTrigger.Invoke(this, EventArgs.Empty);
    this.Timer.Enabled = true;
    this.Counter = 0;
  }

  private void HandleTimer(object? source, EventArgs e)
  {
    HandleTrigger();
  }
    
  public void Increment()
  {
    this.Counter++;
    if (this.MaxCount <= this.Counter)
    {
      HandleTrigger();
    }
  }
}