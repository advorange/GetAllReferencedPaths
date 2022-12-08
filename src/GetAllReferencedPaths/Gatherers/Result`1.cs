using System.Diagnostics.CodeAnalysis;

namespace GetAllReferencedPaths.Gatherers;

public record Result<T>(
	string? ErrorMessage,
	[property: MemberNotNullWhen(true, nameof(Result<object>.Value))]
	[property: MemberNotNullWhen(false, nameof(Result<object>.ErrorMessage))]
	bool IsSuccess,
	T? Value
)
{
	public Result(T value) : this(null, true, value)
	{
	}

	public Result(string error) : this(error, false, default)
	{
	}
}