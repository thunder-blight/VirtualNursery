import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import { Plant } from "../types/Plant"
import "../App.css"

function Home() {
  const [plants, setPlants] = useState<Plant[]>([])
  const [search, setSearch] = useState<string>("")
  const [loading, setLoading] = useState<boolean>(true)
  const [error, setError] = useState<string | null>(null)
  const navigate = useNavigate()

  useEffect(() => {
    fetch("https://localhost:7288/api/plants")
      .then((res: Response) => {
        if (!res.ok) throw new Error("Failed to fetch plants")
        return res.json()
      })
      .then((data: Plant[]) => {
        setPlants(data)
        setLoading(false)
      })
      .catch((err: Error) => {
        setError(err.message)
        setLoading(false)
      })
  }, [])

  const filtered = plants.filter(p =>
    p.name.toLowerCase().includes(search.toLowerCase())
  )

  if (loading) return <div className="status">Loading plants...</div>
  if (error) return <div className="status error">Error: {error}</div>

  return (
    <div className="app">
      <header>
        <h1>🌿 Virtual Nursery</h1>
        <p className="subtitle">{plants.length} plants in the catalog</p>
      </header>
      <div className="search-bar">
        <input
          type="text"
          placeholder="Search plants..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="search-input"
        />
        {search && (
          <button className="search-clear" onClick={() => setSearch("")}>✕</button>
        )}
      </div>
      {filtered.length === 0 ? (
        <p className="status">No plants match "{search}".</p>
      ) : (
        <div className="plant-grid">
          {filtered.map((plant: Plant) => (
            <button
              key={plant.plantID}
              className="plant-button"
              onClick={() => navigate(`/plant/${plant.plantID}`)}
            >
              <div className="plant-button-left">
                <span className="plant-name">{plant.name}</span>
                <span className="plant-type">{plant.type}</span>
              </div>
              <span className="plant-chevron">›</span>
            </button>
          ))}
        </div>
      )}
    </div>
  )
}

export default Home