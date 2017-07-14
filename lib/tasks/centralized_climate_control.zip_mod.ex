defmodule Mix.Tasks.ZipMod do
  use Mix.Task

  @shortdoc "Zips the Mod for Upload."
  @release_dir "release/"
  @modname "CentralizedClimateControl"
  @version "1.1.0"
  @game_version "A17"

  def run(_) do
    path = "./"

    reject_list = [
      "./.git",
      "./.gitignore",
      "./config",
      "./lib",
      "./Misc",
      "./mix.exs",
      "./Source",
      "./release",
      "./_build"
    ]
    
    {:ok, file_list} = File.ls(path)
    
    files = Enum.map(file_list, fn filename -> Path.join(path, filename) end)
        |> Enum.filter(fn(x) -> Enum.member?(reject_list, x) != true end)
        |> Enum.map(fn(x) -> String.split(x, "./") |> Enum.at(1) end)
        |> Enum.map(&String.to_charlist/1)

    IO.puts "Following Directories will be put in the Mod:"
    IO.inspect files

    filename = Enum.join([@release_dir, "[", @game_version, "]", "[", @version, "]", @modname , ".zip"]) |> String.to_charlist
    :zip.create(filename, files)

    IO.puts ["Created File: ", filename]
  end
end