import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import { Plant } from "../types/Plant"
import { Options } from "../types/Options"
import PlantForm from "../components/PlantForm"
import "../App.css"

function CreatePlant() {
  const navigate = useNavigate()
  const [options, setOptions] = useState<Options | null>(null)
  const [loading, setLoading] = useState<boolean>(true)
  const [saving, setSaving] = useState<boolean>(false)
  const [error, setError] = useState<string | null>(null)

  const [form, setForm] = useState({
    name: "",
    type: "",
    lifeCycle: "",
    floweringStatus: false
  })

  useEffect(() => {
    fetch("https://localhost:7288/api/options")
      .then(res => {
        if (!res.ok) throw new Error("Failed to load options")
        return res.json()
      })
      .then((data: Options) => {
        setOptions(data)
        setForm(prev => ({
          ...prev,
          type: data.plantTypes[0],
          lifeCycle: data.lifeCycleTypes[0]
        }))
        setLoading(false)
      })
      .catch((err: Error) => {
        setError(err.message)
        setLoading(false)
      })
  }, [])

  const handleChange = (field: string, value: string | boolean) => {
    setForm(prev => ({ ...prev, [field]: value }))
  }

  const handleSubmit = () => {
    if (!form.name.trim()) {
      setError("Plant name is required.")
      return
    }
    setSaving(true)

    fetch("https://localhost:7288/api/plants", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(form)
    })
      .then(res => {
        if (res.status === 409) throw new Error("A plant with that name already exists.")
        if (!res.ok) throw new Error("Failed to create plant.")
        return res.json()
      })
      .then((created: Plant) => {
        navigate(`/plant/${created.plantID}`)
      })
      .catch((err: Error) => {
        setError(err.message)
        setSaving(false)
      })
  }

  if (loading) return <div className="status">Loading...</div>

  return (
    <div className="app">
      <button className="back-button" onClick={() => navigate("/")}>
        ← Back
      </button>
      {options && (
        <PlantForm
          form={form}
          options={options}
          saving={saving}
          error={error}
          onChange={handleChange}
          onSubmit={handleSubmit}
          onCancel={() => navigate("/")}
          submitLabel="Create Plant"
        />
      )}
    </div>
  )
}

export default CreatePlant