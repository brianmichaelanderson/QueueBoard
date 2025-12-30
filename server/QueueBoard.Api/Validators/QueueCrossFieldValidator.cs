using System.Collections.Generic;
using System.Linq;
using QueueBoard.Api.DTOs;

namespace QueueBoard.Api.Validators
{
    // Validator for simple cross-field rules on QueueDto
    public class QueueCrossFieldValidator
    {
        // Returns false and a descriptive error if Name equals Description (case-insensitive, trimmed).
        public bool Validate(QueueDto dto, out IEnumerable<string> errors)
        {
            var errs = new List<string>();

            var name = dto?.Name?.Trim();
            var desc = dto?.Description?.Trim();

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(desc) && string.Equals(name, desc, System.StringComparison.OrdinalIgnoreCase))
            {
                errs.Add("Name and Description must not be identical.");
            }

            errors = errs;
            return !errs.Any();
        }
    }
}
