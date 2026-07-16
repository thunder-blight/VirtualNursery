import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import "../App.css"
import { Plant } from "../types/Plant"

function Home() {
    const [plants, setPlants] = useState<Plant[]>([])
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

    if (loading) return <div className="status">Loading plants...</div>
    if (error) return <div className="status error">Error: {error}</div>

    return (
        <div className="app">
            <header>
                <h1>🌿 Virtual Nursery</h1>
                <p className="subtitle">{plants.length} plants in the catalog</p>
            </header>
            <div className="plant-grid">
                {plants.map((plant: Plant) => (
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
        </div>
    )
}

export default Home