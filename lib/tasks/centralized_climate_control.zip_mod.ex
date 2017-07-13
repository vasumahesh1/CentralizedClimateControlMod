defmodule Mix.Tasks.ZipMod do
  use Mix.Task

  @shortdoc "Zips the Mod for Upload."
  @modname "release/CentralizedClimateControl"
  @version "1.0.1"

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

    filename = Enum.join([@modname , "_", @version , ".zip"]) |> String.to_charlist
    :zip.create(filename, files)

    IO.puts ["Created File: ", filename]
  end
end