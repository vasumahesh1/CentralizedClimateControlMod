defmodule Mix.Tasks.ZipMod do
  use Mix.Task

  @shortdoc "Zips the Mod for Upload."
  @release_dir "release/"
  @modname "CentralizedClimateControl"
  @version "1.5.0"
  @game_version "B18"

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
    
    copy_dest = Enum.join([@release_dir, @modname, "/"])

    :ok = File.mkdir_p(copy_dest)

    files = Enum.map(file_list, fn filename -> Path.join(path, filename) end)
        |> Enum.filter(fn(x) -> Enum.member?(reject_list, x) != true end)
        |> Enum.map(fn(x) -> String.split(x, "./") |> Enum.at(1) end)
        |> Enum.map(&String.to_charlist/1)

    IO.puts "\nFollowing Directories will be put in the Mod:"
    IO.inspect files

    IO.puts ["\nMoving Files to: ", copy_dest]
    Enum.each(files, &File.cp_r(&1, Enum.join([copy_dest, &1])))

    dest_zip_files = Enum.map(files, &Enum.join([@modname, "/", &1])) |> Enum.map(&String.to_charlist/1)
    
    IO.puts "\nFollowing Directories will be Zipped:"
    IO.inspect dest_zip_files

    filename = Enum.join([@release_dir, "[", @game_version, "]", "[", @version, "]", @modname , ".zip"]) |> String.to_charlist
    IO.puts ["\nCreating File: ", filename]

    {:ok, _} = :zip.create(filename, dest_zip_files, [{:cwd, String.to_charlist(@release_dir) }])
  end
end