# L20n.cs

An implementation of L20n-For-Games in C#, Targeting the Unity Game Engine.

The grammar specs for the "L20n-For-Games" language can be found [here](https://github.com/GlenDC/L20n/blob/master/design/l20n-specs.md).

## L20n.unity

This library can be used for any project that is developed using .NET 2 or higher.
It was however designed specifically to use well within the Unity environment.
The source code for the L20n Unity Plugin can be found at [github.com/GlenDC/L20n.unity](https://github.com/GlenDC/L20n.unity).

## Contributing

Contributions are always welcome.
Ideas, suggestions, Issue Reports are just as good and welcome as code contributions.
The library is finished, meaning that it contains all the functionality that was aimed for,
but that doesn't mean there is no room for improvement. Just keep in mind that the aim
is to keep the library and L20n language as simple as possible,
meaning that we should minimize the amount the user of the language has to learn.

Do you want to contribute, but you have no idea where you can make yourself useful?
Here are some ideas (in no particular order):

+ Currently we have to translate every update frame as there is no way to know,
  wether or not a translation might not be different the next time (due to a callback or so).
  We do currently optimize already the static situations that can easily be optimized into final result.
  But what about all the other situations. Some examples and ideas on how we can further optimize this:
    + Imagine a sentence based on a user variable. Currently that is processing that entire translation tree
      every translate call, as we have at this point no idea wether or not that variable has changed.
      Instead we could make it so that external variables have a kind of dirty flag, that mark wether or not that variable needs to recompute or not. Another idea is to compute a hash based on the content. Both of it relies extra work for the developer though;
    + Global variables are currently also not optimized away. But why not? Many of them will be static. So there should be a way to also cache the computed results of objects using global values.
    + And lastly, I'm not sure if I'm really all that worried about the current runtime cost of the library.
      I think it still will be only a dwarf among all the other heavy tasks, such as rendering in games.
      After some profiling it seems to be a neglectable cost, but I suppose only a big game project on a
      resource-limited platform could tell.
+ The entire codebase contains documentation, both inline and as XML comments. Writing good documentation however, is hard.
  There is definitely room for improvement here, so feel free to correct my English (not my native language) and
  modify the existing documentation to make it more clear, or add missing information.
+ There are lot of unit test and I tried to apply the TDD principle as much as possible.
  Do you see any missing unit-tests? Feel free to add them.
+ This lib can be used in any .NET 2+ environment, but is specifically written to be used in a commercial Unity package.
  Unity supports a lot of platforms, and some of them really need performant code.
  Localization isn't that important compared to other areas and should therefore be as invisible as possible,
  in terms of performance. I'm 99% sure there is room for improvement in this area, so if you're good
  at writing performant C# code, feel free to contribute and others will be grateful for it.
  Also see the first idea I mentioned for more inspiration related to this more general contribution idea.
+ It might be useful to be able to transpile the L20n to C# byte code, so that this can be loaded at initialization.
  This way the entire parsing/AST code can be skipped in production code. I'm not sure if this is really something
  we want to do, as the parsing time seems neglectable, but it's an idea and feel free to work on it if
  you feel like it.

Some guidelines for contributions:

+ Please respect the coding standards already present in the code base.
  I'm not a fan of these coding standards, but I do aim for consistency.
+ When adding new "special cases" or features, or any other modification,
  make sure to add unit-tests to reflect those changes. Never remove existing test cases
  unless you have a very good reason to do so.
+ In order to submit your contribution, simply fork this repository and make a pull request.

## Credits

+ [GlenDC](https://github.com/GlenDC) (developer/maintainer);

MIT Licensed, read license in this directory for more information.
