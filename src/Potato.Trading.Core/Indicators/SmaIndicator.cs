using System;
using System.Collections.Generic;
using System.Linq;

namespace Potato.Trading.Core.Indicators;

public class SmaIndicator
{
    private readonly int _period;
    private readonly Queue<decimal> _values = new();

    public SmaIndicator(int period)
    {
        if (period <= 0) throw new ArgumentOutOfRangeException(nameof(period));
        _period = period;
    }

    public decimal Calculate(decimal newValue)
    {
        _values.Enqueue(newValue);

        if (_values.Count > _period)
        {
            _values.Dequeue();
        }

        if (_values.Count < _period)
        {
            return 0; // Not enough data
        }

        return _values.Average();
    }
    
    public bool IsReady => _values.Count >= _period;
}
